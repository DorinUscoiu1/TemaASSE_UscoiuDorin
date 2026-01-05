using System;
using System.Collections.Generic;

namespace Domain.Models
{
    /// <summary>
    /// Represents an author of a book.
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Gets or sets the unique identifier for the author.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the author.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the author.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the collection of books written by this author.
        /// </summary>
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
