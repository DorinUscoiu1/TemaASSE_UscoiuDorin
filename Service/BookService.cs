// <copyright file="BookService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Repositories;
    using Data.Validators;
    using Domain.Models;
    using FluentValidation;

    /// <summary>
    /// Service implementation for book operations with business rule validation.
    /// Enforces: domain hierarchy, max domains per book, ancestor-descendant validation.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBook bookRepository;
        private readonly IBookDomain bookDomainRepository;
        private readonly LibraryConfiguration configRepository;
        private readonly IValidator<Book> bookValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookService"/> class.
        /// </summary>
        /// <param name="bookRepository">The book repository.</param>
        /// <param name="bookDomainRepository">The book domain repository.</param>
        /// <param name="configRepository">The library configuration repository.</param>
        public BookService(
            IBook bookRepository,
            IBookDomain bookDomainRepository,
            LibraryConfiguration configRepository)
        {
            this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            this.bookDomainRepository = bookDomainRepository ?? throw new ArgumentNullException(nameof(bookDomainRepository));
            this.configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            this.bookValidator = new BookValidator();
        }

        /// <summary>
        /// Gets all books.
        /// </summary>
        public IEnumerable<Book> GetAllBooks()
        {
            return this.bookRepository.GetAll();
        }

        /// <summary>
        /// Gets a book by ID.
        /// </summary>
        public Book GetBookById(int bookId)
        {
            return this.bookRepository.GetById(bookId);
        }

        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        public IEnumerable<Book> GetBooksByAuthor(int authorId)
        {
            return this.bookRepository.GetBooksByAuthor(authorId);
        }

        /// <summary>
        /// Gets books by domain, including inherited domains from hierarchy.
        /// If a book is in "Algoritmi", it's also retrieved from "Informatica" and "Stiinta".
        /// </summary>
        public IEnumerable<Book> GetBooksByDomain(int domainId)
        {
            var domain = this.bookDomainRepository.GetById(domainId);
            if (domain == null)
            {
                return Enumerable.Empty<Book>();
            }

            // Get all descendant domains
            var domainIds = new List<int> { domainId };
            this.GetDescendantDomainIds(domainId, domainIds);

            // Return books from domain and all descendants
            var booksInDomain = this.bookRepository.GetBooksByDomain(domainId);
            var booksInDescendants = domainIds
                .Skip(1)
                .SelectMany(did => this.bookRepository.GetBooksByDomain(did))
                .Distinct();

            return booksInDomain.Concat(booksInDescendants).Distinct();
        }

        /// <summary>
        /// Gets books available for borrowing.
        /// </summary>
        public IEnumerable<Book> GetAvailableBooks()
        {
            return this.bookRepository.GetAvailableBooks();
        }

        /// <summary>
        /// Gets books in a domain including all subdomain books.
        /// If a book is in "Algoritmi", it's also retrieved from "Informatica" and "Stiinta".
        /// </summary>
        public IEnumerable<Book> GetBooksInDomain(int domainId)
        {
            var domain = this.bookDomainRepository.GetById(domainId);
            if (domain == null)
            {
                return Enumerable.Empty<Book>();
            }

            // Get all descendant domain IDs
            var descendantIds = new List<int>();
            this.GetDescendantDomainIds(domainId, descendantIds);
            descendantIds.Add(domainId);

            // Get all books in domain and subdomains
            var allBooks = new List<Book>();
            foreach (var id in descendantIds)
            {
                var booksInDomain = this.bookRepository.GetBooksByDomain(id);
                allBooks.AddRange(booksInDomain);
            }

            return allBooks.GroupBy(b => b.Id).Select(g => g.First());
        }

        /// <summary>
        /// Validates if book can be loaned based on availability.
        /// Rule: At least 10% of loanable copies must remain available.
        /// </summary>
        public bool CanBorrowBook(int bookId)
        {
            var book = this.bookRepository.GetById(bookId);
            if (book == null)
            {
                return false;
            }

            return book.CanBeLoanable();
        }

        /// <summary>
        /// Gets books sorted by availability (most available first).
        /// </summary>
        public IEnumerable<Book> GetBooksOrderedByAvailability()
        {
            var availableBooks = this.bookRepository.GetAvailableBooks();
            return availableBooks.OrderByDescending(b => b.GetAvailableCopies());
        }

        /// <summary>
        /// Gets books from specific domain without subdomains.
        /// </summary>
        public IEnumerable<Book> GetBooksDirectlyInDomain(int domainId)
        {
            var domain = this.bookDomainRepository.GetById(domainId);
            if (domain == null)
            {
                return Enumerable.Empty<Book>();
            }

            return this.bookRepository.GetBooksByDomain(domainId);
        }

        /// <summary>
        /// Checks if book with ISBN exists.
        /// </summary>
        public bool IsbnExists(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                return false;
            }

            var book = this.bookRepository.GetByISBN(isbn);
            return book != null;
        }

        /// <summary>
        /// Gets total books count.
        /// </summary>
        public int GetTotalBooksCount()
        {
            return this.bookRepository.GetAll().Count();
        }

        /// <summary>
        /// Gets total available copies across all books.
        /// </summary>
        public int GetTotalAvailableCopies()
        {
            var allBooks = this.bookRepository.GetAll();
            return allBooks.Sum(b => b.GetAvailableCopies());
        }

        /// <summary>
        /// Gets books with no available copies (all borrowed).
        /// </summary>
        public IEnumerable<Book> GetBooksWithNoCopiesAvailable()
        {
            var allBooks = this.bookRepository.GetAll();
            return allBooks.Where(b => b.GetAvailableCopies() <= 0);
        }

        /// <summary>
        /// Gets books that are only available in reading room.
        /// </summary>
        public IEnumerable<Book> GetReadingRoomOnlyBooks()
        {
            var allBooks = this.bookRepository.GetAll();
            return allBooks.Where(b => b.TotalCopies == b.ReadingRoomOnlyCopies);
        }

        /// <summary>
        /// Creates a new book with comprehensive domain validation.
        /// Rule 1: Max DOMENII domains
        /// Rule 2: No explicit ancestor-descendant relationships.
        /// Rule 3: Total copies must be positive
        /// Rule 4: Book must belong to at least one domain
        /// </summary>
        public void CreateBook(Book book)
        {
            // Validation 1: Book cannot be null
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book), "Book cannot be null.");
            }

            // Validation 2: Validate using FluentValidation
            var validationResult = this.bookValidator.Validate(book);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }

            // Validation 3: Book must belong to at least one domain
            if (book.Domains == null || book.Domains.Count == 0)
            {
                throw new ArgumentException("Book must belong to at least one domain.", nameof(book.Domains));
            }

            // Validation 4: Check for duplicate ISBN
            if (!string.IsNullOrWhiteSpace(book.ISBN))
            {
                var existingBook = this.bookRepository.GetByISBN(book.ISBN);
                if (existingBook != null)
                {
                    throw new InvalidOperationException($"Book with ISBN '{book.ISBN}' already exists.");
                }
            }

            // Validation 5: Validate domain constraints
            if (!this.ValidateBookDomains(book))
            {
                throw new InvalidOperationException(
                    "Book domain constraints violated: exceeds maximum domains or contains ancestor-descendant relationship.");
            }

            try
            {
                this.bookRepository.Add(book);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create book: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates a book.
        /// </summary>
        public void UpdateBook(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            // Validate using FluentValidation
            var validationResult = this.bookValidator.Validate(book);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }

            if (!this.ValidateBookDomains(book))
            {
                throw new InvalidOperationException("Book domain constraints violated.");
            }

            this.bookRepository.Update(book);
        }

        /// <summary>
        /// Deletes a book.
        /// </summary>
        public void DeleteBook(int bookId)
        {
            this.bookRepository.Delete(bookId);
        }

        /// <summary>
        /// Gets available copies count.
        /// </summary>
        public int GetAvailableCopies(int bookId)
        {
            var book = this.bookRepository.GetById(bookId);
            return book?.GetAvailableCopies() ?? 0;
        }

        /// <summary>
        /// Validates book domain constraints.
        /// 1. Max DOMENII domains
        /// 2. No ancestor-descendant explicit relationships.
        /// </summary>
        public bool ValidateBookDomains(Book book)
        {
            if (book?.Domains == null)
            {
                return false;
            }

            var config = this.configRepository;

            if (book.Domains.Count > config.MaxDomainsPerBook)
            {
                return false;
            }

            var domainList = book.Domains.ToList();
            for (int i = 0; i < domainList.Count; i++)
            {
                for (int j = i + 1; j < domainList.Count; j++)
                {
                    if (this.IsAncestor(domainList[i].Id, domainList[j].Id) || 
                        this.IsAncestor(domainList[j].Id, domainList[i].Id))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if ancestorId is an ancestor of descendantId.
        /// </summary>
        private bool IsAncestor(int potentialAncestorId, int potentialDescendantId)
        {
            var currentDomain = this.bookDomainRepository.GetById(potentialDescendantId);

            while (currentDomain?.ParentDomainId.HasValue == true)
            {
                if (currentDomain.ParentDomainId == potentialAncestorId)
                {
                    return true;
                }

                currentDomain = this.bookDomainRepository.GetById(currentDomain.ParentDomainId.Value);
            }

            return false;
        }

        /// <summary>
        /// Gets all descendant domain IDs recursively.
        /// </summary>
        private void GetDescendantDomainIds(int parentDomainId, List<int> result)
        {
            var subdomains = this.bookDomainRepository.GetSubdomains(parentDomainId);
            foreach (var subdomain in subdomains)
            {
                result.Add(subdomain.Id);
                this.GetDescendantDomainIds(subdomain.Id, result);
            }
        }
    }
}