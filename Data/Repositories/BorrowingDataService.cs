using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Data.Repositories
{
    /// <summary>
    /// Repository implementation for Borrowing-specific operations.
    /// </summary>
    public class BorrowingDataService : IBorrowing
    {
        private readonly LibraryDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BorrowingDataService"/> class.
        /// </summary>
        /// <param name="context">The library database context.</param>
        public BorrowingDataService(LibraryDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets active borrowings for a specific reader.
        /// </summary>
        /// <param name="readerId">The reader identifier.</param>
        /// <returns>A collection of active borrowings for the specified reader.</returns>
        public IEnumerable<Domain.Models.Borrowing> GetActiveBorrowingsByReader(int readerId)
        {
            return this.context.Borrowings
                .Where(b => b.ReaderId == readerId && b.IsActive)
                .ToList();
        }

        /// <summary>
        /// Gets overdue borrowings.
        /// </summary>
        /// <returns>A collection of borrowings past their due date.</returns>
        public IEnumerable<Domain.Models.Borrowing> GetOverdueBorrowings()
        {
            return this.context.Borrowings
                .Where(b => b.IsActive && b.DueDate < DateTime.Now)
                .ToList();
        }

        /// <summary>
        /// Gets borrowings by book ID.
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns>A collection of borrowings for the specified book.</returns>
        public IEnumerable<Domain.Models.Borrowing> GetBorrowingsByBook(int bookId)
        {
            return this.context.Borrowings
                .Where(b => b.BookId == bookId)
                .ToList();
        }

        /// <summary>
        /// Gets borrowings within a date range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>A collection of borrowings within the specified date range.</returns>
        public IEnumerable<Domain.Models.Borrowing> GetBorrowingsByDateRange(DateTime startDate, DateTime endDate)
        {
            return this.context.Borrowings
                .Where(b => b.BorrowingDate >= startDate && b.BorrowingDate <= endDate)
                .ToList();
        }
    }
}