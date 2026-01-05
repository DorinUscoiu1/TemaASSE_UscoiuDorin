// <copyright file="BorrowingService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Repositories;
    using Domain.Models;

    /// <summary>
    /// Service implementation for borrowing operations with business rules validation.
    /// Enforces all constraints defined in LibraryConfiguration.
    /// </summary>
    public class BorrowingService : IBorrowingService
    {
        private readonly IBorrowing borrowingRepository;
        private readonly IBook bookRepository;
        private readonly IReader readerRepository;
        private readonly LibraryConfiguration configRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BorrowingService"/> class.
        /// </summary>
        /// <param name="borrowingRepository">The borrowing repository.</param>
        /// <param name="bookRepository">The book repository.</param>
        /// <param name="readerRepository">The reader repository.</param>
        /// <param name="configRepository">The library configuration repository.</param>
        public BorrowingService(
            IBorrowing borrowingRepository,
            IBook bookRepository,
            IReader readerRepository,
            LibraryConfiguration configRepository)
        {
            this.borrowingRepository = borrowingRepository;
            this.bookRepository = bookRepository;
            this.readerRepository = readerRepository;
            this.configRepository = configRepository;
        }

        /// <summary>
        /// Attempts to borrow a book with comprehensive business rule validation.
        /// </summary>
        public void BorrowBook(int readerId, int bookId, int borrowingDays)
        {
            var reader = this.readerRepository.GetById(readerId);
            var book = this.bookRepository.GetById(bookId);

            if (reader == null)
            {
                throw new InvalidOperationException("Reader not found.");
            }

            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            if (!this.CanBorrowBook(readerId, bookId))
            {
                throw new InvalidOperationException("Reader cannot borrow this book. Business rules violated.");
            }
        }

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        public void ReturnBook(int borrowingId)
        {
            var borrowing = new Borrowing { Id = borrowingId };
            if (borrowing == null)
            {
                throw new InvalidOperationException("Borrowing record not found.");
            }

            borrowing.ReturnDate = DateTime.Now;
            borrowing.IsActive = false;
        }

        /// <summary>
        /// Extends a borrowing period if extension limit not exceeded.
        /// Maximum extension days (LIM) in last 3 months.
        /// </summary>
        public void ExtendBorrowing(int borrowingId, int extensionDays)
        {
            var borrowing = new Borrowing { Id = borrowingId };
            if (borrowing == null)
            {
                throw new InvalidOperationException("Borrowing record not found.");
            }

            var maxExtensionDays = this.configRepository.MaxExtensionDays;
            if (borrowing.TotalExtensionDays + extensionDays > maxExtensionDays)
            {
                throw new InvalidOperationException($"Extension would exceed limit of {maxExtensionDays} days.");
            }

            borrowing.DueDate = borrowing.DueDate.AddDays(extensionDays);
            borrowing.TotalExtensionDays += extensionDays;
            borrowing.LastExtensionDate = DateTime.Now;
        }

        /// <summary>
        /// Gets all active borrowings for a reader.
        /// </summary>
        public IEnumerable<Borrowing> GetActiveBorrowings(int readerId)
        {
            return this.borrowingRepository.GetActiveBorrowingsByReader(readerId);
        }

        /// <summary>
        /// Gets overdue borrowings.
        /// </summary>
        public IEnumerable<Borrowing> GetOverdueBorrowings()
        {
            return this.borrowingRepository.GetOverdueBorrowings();
        }

        /// <summary>
        /// Validates all business rules for borrowing a book.
        /// </summary>
        public bool CanBorrowBook(int readerId, int bookId)
        {
            var config = this.configRepository;
            var reader = this.readerRepository.GetById(readerId);
            var book = this.bookRepository.GetById(bookId);

            if (reader == null || book == null)
            {
                return false;
            }

            if (book.GetAvailableCopies() <= 0)
            {
                return false;
            }

            var availablePercentage = (decimal)book.GetAvailableCopies() / book.TotalCopies;
            if (availablePercentage < config.MinAvailablePercentage)
            {
                return false;
            }

            var activeBorrowings = this.borrowingRepository.GetActiveBorrowingsByReader(readerId);
            var maxBooks = reader.IsStaff ? config.MaxBooksPerPeriod * 2 : config.MaxBooksPerPeriod;
            if (activeBorrowings.Count() >= maxBooks)
            {
                return false;
            }

            // Rule 4: Check books per domain in last L months
            var domainLimitMonths = config.DomainLimitMonths;
            var lastMonthBorrowings = this.borrowingRepository.GetBorrowingsByDateRange(
                DateTime.Now.AddMonths(-domainLimitMonths),
                DateTime.Now);

            foreach (var domain in book.Domains)
            {
                var domainBooksCount = lastMonthBorrowings
                    .Count(b => b.Book.Domains.Any(d => d.Id == domain.Id));

                var maxDomainBooks = reader.IsStaff ? config.MaxBooksPerDomain * 2 : config.MaxBooksPerDomain;
                if (domainBooksCount >= maxDomainBooks)
                    return false;
            }

            // Rule 5: Check DELTA - min days between consecutive borrows
            var lastBorrowing = this.borrowingRepository.GetBorrowingsByBook(bookId)
                .OrderByDescending(b => b.BorrowingDate)
                .FirstOrDefault();

            if (lastBorrowing != null && lastBorrowing.ReturnDate.HasValue)
            {
                var daysSinceReturn = (DateTime.Now - lastBorrowing.ReturnDate.Value).Days;
                var minDaysBetweenBorrows = reader.IsStaff 
                    ? config.MinDaysBetweenBorrows / 2 
                    : config.MinDaysBetweenBorrows;

                if (daysSinceReturn < minDaysBetweenBorrows)
                    return false;
            }

            // Rule 6: Check max books per day
            var todayBorrowings = this.borrowingRepository.GetBorrowingsByDateRange(
                DateTime.Now.Date,
                DateTime.Now);

            var maxBooksPerDay = reader.IsStaff ? int.MaxValue : config.MaxBooksPerDay; // Staff ignores NCZ
            if (todayBorrowings.Count() >= maxBooksPerDay)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the count of active borrowings for a reader.
        /// </summary>
        public int GetActiveBorrowingCount(int readerId)
        {
            return this.borrowingRepository.GetActiveBorrowingsByReader(readerId).Count();
        }
    }
}