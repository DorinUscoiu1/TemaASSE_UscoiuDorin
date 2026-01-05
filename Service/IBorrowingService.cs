// <copyright file="IBorrowingService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Service interface for borrowing operations with business rules validation.
    /// </summary>
    public interface IBorrowingService
    {
        /// <summary>
        /// Attempts to borrow a book for a reader with all business rule validations.
        /// </summary>
        void BorrowBook(int readerId, int bookId, int borrowingDays);

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        void ReturnBook(int borrowingId);

        /// <summary>
        /// Extends a borrowing period if allowed.
        /// </summary>
        void ExtendBorrowing(int borrowingId, int extensionDays);

        /// <summary>
        /// Gets all active borrowings for a reader.
        /// </summary>
        IEnumerable<Borrowing> GetActiveBorrowings(int readerId);

        /// <summary>
        /// Gets overdue borrowings.
        /// </summary>
        IEnumerable<Borrowing> GetOverdueBorrowings();

        /// <summary>
        /// Validates if a reader can borrow a book.
        /// </summary>
        bool CanBorrowBook(int readerId, int bookId);

        /// <summary>
        /// Gets the count of active borrowings for a reader.
        /// </summary>
        int GetActiveBorrowingCount(int readerId);
    }
}