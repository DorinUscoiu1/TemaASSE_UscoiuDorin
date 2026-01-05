// <copyright file="BookService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Repositories;
    using Domain.Models;

    /// <summary>
    /// Service implementation for book operations with business rule validation.
    /// Enforces: domain hierarchy, max domains per book, ancestor-descendant validation.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBook bookRepository;
        private readonly IBookDomain bookDomainRepository;
        private readonly LibraryConfiguration configRepository;

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
            this.bookRepository = bookRepository;
            this.bookDomainRepository = bookDomainRepository;
            this.configRepository = configRepository;
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
        /// If a book is in "Algoritmi", it's also in "Informatica" and "Stiinta".
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
        /// Creates a new book with comprehensive domain validation.
        /// Rule 1: Max DOMENII domains
        /// Rule 2: No explicit ancestor-descendant relationships.
        /// </summary>
        public void CreateBook(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            if (string.IsNullOrWhiteSpace(book.Title))
            {
                throw new ArgumentException("Book title is required.");
            }

            if (!this.ValidateBookDomains(book))
            {
                throw new InvalidOperationException("Book domain constraints violated.");
            }

            this.bookRepository.Add(book);
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
            var config = this.configRepository;

            if (book.Domains.Count > config.MaxDomainsPerBook)
            {
                return false;
            }

            var domainList = book.Domains.ToList();
            foreach (var domain1 in domainList)
            {
                foreach (var domain2 in domainList)
                {
                    if (domain1.Id == domain2.Id)
                    {
                        continue;
                    }

                    if (this.IsAncestor(domain1.Id, domain2.Id) || this.IsAncestor(domain2.Id, domain1.Id))
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