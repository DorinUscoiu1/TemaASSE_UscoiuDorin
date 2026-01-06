// <copyright file="IAuthorService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Service interface for author operations with business rule validation.
    /// </summary>
    public interface IAuthorService
    {
        /// <summary>
        /// Gets all authors.
        /// </summary>
        IEnumerable<Author> GetAllAuthors();

        /// <summary>
        /// Gets an author by ID.
        /// </summary>
        Author GetAuthorById(int authorId);

        /// <summary>
        /// Gets authors by first name.
        /// </summary>
        IEnumerable<Author> GetAuthorsByFirstName(string firstName);

        /// <summary>
        /// Gets authors by last name.
        /// </summary>
        IEnumerable<Author> GetAuthorsByLastName(string lastName);

        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        IEnumerable<Book> GetBooksByAuthor(int authorId);

        /// <summary>
        /// Creates a new author with validation.
        /// </summary>
        void CreateAuthor(Author author);

        /// <summary>
        /// Updates an author.
        /// </summary>
        void UpdateAuthor(Author author);

        /// <summary>
        /// Deletes an author.
        /// </summary>
        void DeleteAuthor(int authorId);
    }
}