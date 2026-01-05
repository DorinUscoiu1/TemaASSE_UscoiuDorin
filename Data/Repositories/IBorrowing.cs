// <copyright file="IBorrowing.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Repository interface for Borrowing-specific operations.
    /// </summary>
    public interface IBorrowing
    {
        /// <summary>
        /// Gets active borrowings for a specific reader.
        /// </summary>
        IEnumerable<Borrowing> GetActiveBorrowingsByReader(int readerId);

        /// <summary>
        /// Gets overdue borrowings.
        /// </summary>
        IEnumerable<Borrowing> GetOverdueBorrowings();

        /// <summary>
        /// Gets borrowings by book ID.
        /// </summary>
        IEnumerable<Borrowing> GetBorrowingsByBook(int bookId);

        /// <summary>
        /// Gets borrowings within a date range.
        /// </summary>
        IEnumerable<Borrowing> GetBorrowingsByDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Adds a new borrowing.
        /// </summary>
        void Add(Borrowing borrowing);

        /// <summary>
        /// Updates a borrowing.
        /// </summary>
        void Update(Borrowing borrowing);

        /// <summary>
        /// Gets a borrowing by ID.
        /// </summary>
        Borrowing GetById(int id);
    }
}