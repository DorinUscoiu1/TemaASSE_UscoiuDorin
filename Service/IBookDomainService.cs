// <copyright file="IBookDomainService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Service interface for book domain operations with business rule validation.
    /// </summary>
    public interface IBookDomainService
    {
        /// <summary>
        /// Gets all domains.
        /// </summary>
        IEnumerable<BookDomain> GetAllDomains();

        /// <summary>
        /// Gets a domain by ID.
        /// </summary>
        BookDomain GetDomainById(int domainId);

        /// <summary>
        /// Gets root domains (without parent).
        /// </summary>
        IEnumerable<BookDomain> GetRootDomains();

        /// <summary>
        /// Gets subdomains by parent ID.
        /// </summary>
        IEnumerable<BookDomain> GetSubdomains(int parentDomainId);

        /// <summary>
        /// Creates a new domain.
        /// </summary>
        void CreateDomain(BookDomain domain);

        /// <summary>
        /// Updates a domain.
        /// </summary>
        void UpdateDomain(BookDomain domain);

        /// <summary>
        /// Deletes a domain.
        /// </summary>
        void DeleteDomain(int domainId);

        /// <summary>
        /// Gets all ancestor domains for a domain.
        /// </summary>
        IEnumerable<BookDomain> GetAncestorDomains(int domainId);

        /// <summary>
        /// Gets all descendant domains for a domain.
        /// </summary>
        IEnumerable<BookDomain> GetDescendantDomains(int domainId);
    }
}