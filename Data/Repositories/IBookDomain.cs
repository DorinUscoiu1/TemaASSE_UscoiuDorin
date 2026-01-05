// <copyright file="IBookDomain.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Repository interface for BookDomain-specific operations.
    /// </summary>
    public interface IBookDomain
    {
        /// <summary>
        /// Gets all domains.
        /// </summary>
        IEnumerable<BookDomain> GetAll();

        /// <summary>
        /// Gets a domain by ID.
        /// </summary>
        BookDomain GetById(int id);

        /// <summary>
        /// Gets root domains (without parent).
        /// </summary>
        IEnumerable<BookDomain> GetRootDomains();

        /// <summary>
        /// Gets subdomains by parent ID.
        /// </summary>
        IEnumerable<BookDomain> GetSubdomains(int parentDomainId);

        /// <summary>
        /// Adds a new domain.
        /// </summary>
        void Add(BookDomain domain);

        /// <summary>
        /// Updates a domain.
        /// </summary>
        void Update(BookDomain domain);

        /// <summary>
        /// Deletes a domain.
        /// </summary>
        void Delete(int id);
    }
}