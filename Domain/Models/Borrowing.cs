using System;

namespace Domain.Models
{
    /// <summary>
    /// Represents a borrowing transaction of a book copy by a reader.
    /// Tracks the borrowing and return dates, extensions, and current status.
    /// </summary>
    public class Borrowing
    {
        /// <summary>
        /// Gets or sets the unique identifier for the borrowing record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the reader who borrowed the book.
        /// </summary>
        public int ReaderId { get; set; }

        /// <summary>
        /// Gets or sets the reader who borrowed the book.
        /// </summary>
        public virtual Reader Reader { get; set; }
        /// <summary>
        /// Gets or sets the ID of the book (for quick reference and filtering).
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the book (for quick reference and filtering).
        /// </summary>
        public virtual Book Book { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the book was borrowed.
        /// </summary>
        public DateTime BorrowingDate { get; set; }

        /// <summary>
        /// Gets or sets the due date for returning the book.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the actual return date (null if not yet returned).
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the total number of days extended (sum of all extensions in the last 3 months).
        /// </summary>
        public int TotalExtensionDays { get; set; }

        /// <summary>
        /// Gets or sets the date of the last extension (for tracking extension limits).
        /// </summary>
        public DateTime? LastExtensionDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this borrowing record is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the initial number of borrowing days.
        /// </summary>
        public int InitialBorrowingDays { get; set; }
    }
}