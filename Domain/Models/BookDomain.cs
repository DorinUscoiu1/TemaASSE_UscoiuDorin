using System;
using System.Collections.Generic;

namespace Domain.Models
{
    /// <summary>
    /// Represents a domain/category of books (e.g., Science, Mathematics).
    /// Domains can have subdomains forming a hierarchical structure.
    /// </summary>
    public class BookDomain
    {
        /// <summary>
        /// Gets or sets the unique identifier for the domain.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the domain.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the domain.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent domain (null if this is a root domain).
        /// </summary>
        public int? ParentDomainId { get; set; }

        /// <summary>
        /// Gets or sets the parent domain (null if this is a root domain).
        /// </summary>
        public virtual BookDomain ParentDomain { get; set; }

        /// <summary>
        /// Gets or sets the collection of subdomains.
        /// </summary>
        public virtual ICollection<BookDomain> Subdomains { get; set; } = new List<BookDomain>();

        /// <summary>
        /// Gets or sets the collection of books in this domain.
        /// </summary>
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}