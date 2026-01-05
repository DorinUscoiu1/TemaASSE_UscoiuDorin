using System;
using System.Collections.Generic;
using Domain.Models;

namespace Data.Repositories
{
    /// <summary>
    /// Repository interface for Borrowing-specific operations.
    /// </summary>
    public interface IBorrowing
    {
        /// <summary>
        /// Gets active borrowings for a specific reader.
        /// </summary>
        IEnumerable<Domain.Models.Borrowing> GetActiveBorrowingsByReader(int readerId);

        /// <summary>
        /// Gets overdue borrowings.
        /// </summary>
        IEnumerable<Domain.Models.Borrowing> GetOverdueBorrowings();

        /// <summary>
        /// Gets borrowings by book ID.
        /// </summary>
        IEnumerable<Domain.Models.Borrowing> GetBorrowingsByBook(int bookId);

        /// <summary>
        /// Gets borrowings within a date range.
        /// </summary>
        IEnumerable<Domain.Models.Borrowing> GetBorrowingsByDateRange(DateTime startDate, DateTime endDate);
    }
}