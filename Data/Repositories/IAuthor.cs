// <copyright file="IAuthor.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Repository interface for Author-specific operations.
    /// </summary>
    public interface IAuthor
    {
        /// <summary>
        /// Gets all authors.
        /// </summary>
        IEnumerable<Author> GetAll();

        /// <summary>
        /// Gets an author by ID.
        /// </summary>
        Author GetById(int id);

        /// <summary>
        /// Gets authors by last name.
        /// </summary>
        IEnumerable<Author> GetByLastName(string lastName);

        /// <summary>
        /// Adds a new author.
        /// </summary>
        void Add(Author author);

        /// <summary>
        /// Updates an author.
        /// </summary>
        void Update(Author author);

        /// <summary>
        /// Deletes an author.
        /// </summary>
        void Delete(int id);
    }
}