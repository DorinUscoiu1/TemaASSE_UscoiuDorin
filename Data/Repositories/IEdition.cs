// <copyright file="IEdition.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Repository interface for Edition-specific operations.
    /// </summary>
    public interface IEdition
    {
        /// <summary>
        /// Gets all editions.
        /// </summary>
        IEnumerable<Edition> GetAll();

        /// <summary>
        /// Gets an edition by ID.
        /// </summary>
        Edition GetById(int id);

        /// <summary>
        /// Gets editions by book ID.
        /// </summary>
        IEnumerable<Edition> GetByBookId(int bookId);

        /// <summary>
        /// Gets editions by publisher.
        /// </summary>
        IEnumerable<Edition> GetByPublisher(string publisher);

        /// <summary>
        /// Adds a new edition.
        /// </summary>
        void Add(Edition edition);

        /// <summary>
        /// Updates an edition.
        /// </summary>
        void Update(Edition edition);

        /// <summary>
        /// Deletes an edition.
        /// </summary>
        void Delete(int id);
    }
}