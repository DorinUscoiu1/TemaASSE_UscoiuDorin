// <copyright file="IBook.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Repository interface for Book-specific operations.
    /// </summary>
    public interface IBook
    {
        /// <summary>
        /// Gets all books.
        /// </summary>
        IEnumerable<Book> GetAll();

        /// <summary>
        /// Gets a book by ID.
        /// </summary>
        Book GetById(int id);

        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        IEnumerable<Book> GetBooksByAuthor(int authorId);

        /// <summary>
        /// Gets books by domain ID.
        /// </summary>
        IEnumerable<Book> GetBooksByDomain(int domainId);

        /// <summary>
        /// Gets books with available copies.
        /// </summary>
        IEnumerable<Book> GetAvailableBooks();

        /// <summary>
        /// Gets books by ISBN.
        /// </summary>
        Book GetByISBN(string isbn);

        /// <summary>
        /// Adds a new book.
        /// </summary>
        void Add(Book book);

        /// <summary>
        /// Updates a book.
        /// </summary>
        void Update(Book book);

        /// <summary>
        /// Deletes a book.
        /// </summary>
        void Delete(int id);
    }
}