using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

// <copyright file="BorrowingDataService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

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
        public IEnumerable<Borrowing> GetActiveBorrowingsByReader(int readerId)
        {
            return this.context.Borrowings
                .Where(b => b.ReaderId == readerId && b.IsActive)
                .ToList();
        }

        /// <summary>
        /// Gets overdue borrowings.
        /// </summary>
        public IEnumerable<Borrowing> GetOverdueBorrowings()
        {
            return this.context.Borrowings
                .Where(b => b.IsActive && b.DueDate < DateTime.Now)
                .ToList();
        }

        /// <summary>
        /// Gets borrowings by book ID.
        /// </summary>
        public IEnumerable<Borrowing> GetBorrowingsByBook(int bookId)
        {
            return this.context.Borrowings
                .Where(b => b.BookId == bookId)
                .ToList();
        }

        /// <summary>
        /// Gets borrowings within a date range.
        /// </summary>
        public IEnumerable<Borrowing> GetBorrowingsByDateRange(DateTime startDate, DateTime endDate)
        {
            return this.context.Borrowings
                .Where(b => b.BorrowingDate >= startDate && b.BorrowingDate <= endDate)
                .ToList();
        }

        /// <summary>
        /// Adds a new borrowing.
        /// </summary>
        public void Add(Borrowing borrowing)
        {
            if (borrowing == null)
            {
                throw new ArgumentNullException(nameof(borrowing));
            }

            this.context.Borrowings.Add(borrowing);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Updates a borrowing.
        /// </summary>
        public void Update(Borrowing borrowing)
        {
            if (borrowing == null)
            {
                throw new ArgumentNullException(nameof(borrowing));
            }

            var existingBorrowing = this.context.Borrowings.Find(borrowing.Id);
            if (existingBorrowing != null)
            {
                existingBorrowing.DueDate = borrowing.DueDate;
                existingBorrowing.ReturnDate = borrowing.ReturnDate;
                existingBorrowing.IsActive = borrowing.IsActive;
                existingBorrowing.TotalExtensionDays = borrowing.TotalExtensionDays;
                existingBorrowing.LastExtensionDate = borrowing.LastExtensionDate;
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets a borrowing by ID.
        /// </summary>
        public Borrowing GetById(int id)
        {
            return this.context.Borrowings.Find(id);
        }
    }
}