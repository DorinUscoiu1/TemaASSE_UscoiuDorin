// <copyright file="BookDataService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Models;

    /// <summary>
    /// Repository implementation for Book-specific operations.
    /// </summary>
    public class BookDataService : IBook
    {
        private readonly LibraryDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookDataService"/> class.
        /// </summary>
        /// <param name="context">The library database context.</param>
        public BookDataService(LibraryDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets all books.
        /// </summary>
        public IEnumerable<Book> GetAll()
        {
            return this.context.Books.ToList();
        }

        /// <summary>
        /// Gets a book by ID.
        /// </summary>
        public Book GetById(int id)
        {
            return this.context.Books.Find(id);
        }

        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        public IEnumerable<Book> GetBooksByAuthor(int authorId)
        {
            return this.context.Books
                .Where(b => b.Authors.Any(a => a.Id == authorId))
                .ToList();
        }

        /// <summary>
        /// Gets books by domain ID.
        /// </summary>
        public IEnumerable<Book> GetBooksByDomain(int domainId)
        {
            return this.context.Books
                .Where(b => b.Domains.Any(d => d.Id == domainId))
                .ToList();
        }

        /// <summary>
        /// Gets books with available copies.
        /// </summary>
        public IEnumerable<Book> GetAvailableBooks()
        {
            return this.context.Books
                .Where(b => b.GetAvailableCopies() > 0)
                .ToList();
        }

        /// <summary>
        /// Gets books by ISBN.
        /// </summary>
        public Book GetByISBN(string isbn)
        {
            return this.context.Books
                .FirstOrDefault(b => b.ISBN == isbn);
        }

        /// <summary>
        /// Adds a new book.
        /// </summary>
        public void Add(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            this.context.Books.Add(book);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Updates a book.
        /// </summary>
        public void Update(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            var existingBook = this.context.Books.Find(book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.ISBN = book.ISBN;
                existingBook.Description = book.Description;
                existingBook.TotalCopies = book.TotalCopies;
                existingBook.ReadingRoomOnlyCopies = book.ReadingRoomOnlyCopies;
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a book.
        /// </summary>
        public void Delete(int id)
        {
            var book = this.context.Books.Find(id);
            if (book != null)
            {
                this.context.Books.Remove(book);
                this.context.SaveChanges();
            }
        }
    }
}