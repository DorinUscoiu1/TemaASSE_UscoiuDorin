// <copyright file="IBookService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Service interface for book operations with business rule validation.
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Gets all books.
        /// </summary>
        IEnumerable<Book> GetAllBooks();

        /// <summary>
        /// Gets a book by ID.
        /// </summary>
        Book GetBookById(int bookId);

        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        IEnumerable<Book> GetBooksByAuthor(int authorId);

        /// <summary>
        /// Gets books by domain (including ancestor domains via hierarchy).
        /// </summary>
        IEnumerable<Book> GetBooksByDomain(int domainId);

        /// <summary>
        /// Gets books available for borrowing.
        /// </summary>
        IEnumerable<Book> GetAvailableBooks();

        /// <summary>
        /// Creates a new book with domain validation.
        /// Validates: max DOMENII domains, no ancestor-descendant relationships.
        /// </summary>
        void CreateBook(Book book, List<int> domainIds);

        /// <summary>
        /// Updates a book.
        /// </summary>
        void UpdateBook(Book book);

        /// <summary>
        /// Deletes a book.
        /// </summary>
        void DeleteBook(int bookId);

        /// <summary>
        /// Gets available copies count for a book.
        /// </summary>
        int GetAvailableCopies(int bookId);

        /// <summary>
        /// Validates book domain constraints.
        /// </summary>
        bool ValidateBookDomains(Book book);
    }
}