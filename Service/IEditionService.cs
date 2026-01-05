// <copyright file="IEditionService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Service interface for edition operations.
    /// </summary>
    public interface IEditionService
    {
        /// <summary>
        /// Gets all editions.
        /// </summary>
        IEnumerable<Edition> GetAllEditions();

        /// <summary>
        /// Gets an edition by ID.
        /// </summary>
        Edition GetEditionById(int editionId);

        /// <summary>
        /// Gets editions by book ID.
        /// </summary>
        IEnumerable<Edition> GetEditionsByBook(int bookId);

        /// <summary>
        /// Gets editions by publisher.
        /// </summary>
        IEnumerable<Edition> GetEditionsByPublisher(string publisher);

        /// <summary>
        /// Creates a new edition.
        /// </summary>
        void CreateEdition(Edition edition);

        /// <summary>
        /// Updates an edition.
        /// </summary>
        void UpdateEdition(Edition edition);

        /// <summary>
        /// Deletes an edition.
        /// </summary>
        void DeleteEdition(int editionId);
    }
}