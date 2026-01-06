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

            // Create and persist borrowing record
            var borrowing = new Borrowing
            {
                ReaderId = readerId,
                BookId = bookId,
                BorrowingDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(borrowingDays),
                IsActive = true,
                InitialBorrowingDays = borrowingDays,
                TotalExtensionDays = 0
            };

            try
            {
                this.borrowingRepository.Add(borrowing);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create borrowing record: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates borrowing records for multiple books with comprehensive validation.
        /// Validates: max books per request, domain diversity (3+ books from 2+ domains),
        /// daily limits, period limits, and all individual book constraints.
        /// </summary>
        public void CreateBorrowings(int readerId, List<int> bookIds, DateTime borrowingDate, int daysToBorrow)
        {
            var reader = this.readerRepository.GetById(readerId);
            if (reader == null)
            {
                throw new InvalidOperationException("Reader not found.");
            }

            var config = this.configRepository;

            // Validation 1: Check max books per request (C)
            var maxBooksPerRequest = reader.IsStaff
                ? config.MaxBooksPerRequest * 2
                : config.MaxBooksPerRequest;

            if (bookIds.Count > maxBooksPerRequest)
            {
                throw new InvalidOperationException(
                    $"Cannot borrow more than {maxBooksPerRequest} books at once.");
            }

            // Validation 2: Check domain diversity for 3+ books
            if (bookIds.Count >= 3)
            {
                var books = new List<Book>();
                foreach (var bookId in bookIds)
                {
                    var book = this.bookRepository.GetById(bookId);
                    if (book == null)
                    {
                        throw new InvalidOperationException($"Book {bookId} not found.");
                    }
                    books.Add(book);
                }

                var distinctDomains = books
                    .SelectMany(b => b.Domains)
                    .Select(d => d.Id)
                    .Distinct()
                    .Count();

                if (distinctDomains < 2)
                {
                    throw new InvalidOperationException(
                        "When borrowing 3 or more books, they must be from at least 2 different domains.");
                }
            }

            // Validation 3: Check daily limit (NCZ)
            var maxBooksPerDay = reader.IsStaff ? int.MaxValue : config.MaxBooksPerDay;
            var todayBorrowings = this.borrowingRepository.GetBorrowingsByDateRange(
                borrowingDate.Date,
                borrowingDate);

            if (todayBorrowings.Count() + bookIds.Count > maxBooksPerDay)
            {
                throw new InvalidOperationException(
                    $"Cannot borrow more than {maxBooksPerDay} books per day.");
            }

            // Validation 4: Check period limit (NMC)
            var periodDays = config.BorrowingPeriodDays;
            var periodStart = borrowingDate.AddDays(-periodDays);
            var borrowingsInPeriod = this.borrowingRepository.GetBorrowingsByDateRange(
                periodStart,
                borrowingDate);

            var maxBooksInPeriod = reader.IsStaff
                ? config.MaxBooksPerPeriod * 2
                : config.MaxBooksPerPeriod;

            if (borrowingsInPeriod.Count() + bookIds.Count > maxBooksInPeriod)
            {
                throw new InvalidOperationException(
                    $"Cannot borrow more than {maxBooksInPeriod} books in {periodDays} days.");
            }

            // Validation 5: Create borrowing for each book with individual validations
            foreach (var bookId in bookIds)
            {
                if (!this.CanBorrowBook(readerId, bookId))
                {
                    throw new InvalidOperationException(
                        $"Cannot borrow book {bookId}. Business rules violated.");
                }

                var borrowing = new Borrowing
                {
                    ReaderId = readerId,
                    BookId = bookId,
                    BorrowingDate = borrowingDate,
                    DueDate = borrowingDate.AddDays(daysToBorrow),
                    IsActive = true,
                    InitialBorrowingDays = daysToBorrow,
                    TotalExtensionDays = 0
                };

                try
                {
                    this.borrowingRepository.Add(borrowing);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create borrowing record for book {bookId}: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        public void ReturnBorrowing(int borrowingId, DateTime returnDate)
        {
            var borrowing = this.borrowingRepository.GetById(borrowingId);
            if (borrowing == null)
            {
                throw new InvalidOperationException("Borrowing record not found.");
            }

            if (!borrowing.IsActive)
            {
                throw new InvalidOperationException("This borrowing record is already returned.");
            }

            borrowing.ReturnDate = returnDate;
            borrowing.IsActive = false;

            try
            {
                this.borrowingRepository.Update(borrowing);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to return borrowing record: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Extends a borrowing period if allowed.
        /// </summary>
        public void ExtendBorrowing(int borrowingId, int extensionDays, DateTime extensionDate)
        {
            var borrowing = this.borrowingRepository.GetById(borrowingId);
            if (borrowing == null)
            {
                throw new InvalidOperationException("Borrowing record not found.");
            }

            if (!borrowing.IsActive)
            {
                throw new InvalidOperationException("Cannot extend a returned borrowing record.");
            }

            var reader = this.readerRepository.GetById(borrowing.ReaderId);
            if (reader == null)
            {
                throw new InvalidOperationException("Reader not found.");
            }

            var config = this.configRepository;
            var maxExtensionDays = config.MaxExtensionDays;

            // Validation 1: Check total extension limit
            if (borrowing.TotalExtensionDays + extensionDays > maxExtensionDays)
            {
                throw new InvalidOperationException(
                    $"Extension would exceed limit of {maxExtensionDays} days.");
            }

            // Validation 2: Verify book is still available
            var book = this.bookRepository.GetById(borrowing.BookId);
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            if (book.GetAvailableCopies() <= 0)
            {
                throw new InvalidOperationException("Book is no longer available for extension.");
            }

            // Validation 3: Check minimum available percentage
            var availablePercentage = (double)book.GetAvailableCopies() / book.TotalCopies;
            if (availablePercentage < config.MinAvailablePercentage)
            {
                throw new InvalidOperationException(
                    "Cannot extend borrowing. Book availability below minimum threshold.");
            }

            // Validation 4: Check 3-month extension limit (LIM in last 3 months)
            var threeMonthsAgo = extensionDate.AddMonths(-3);
            var borrowingsInLastThreeMonths = this.borrowingRepository.GetBorrowingsByDateRange(
                threeMonthsAgo,
                extensionDate);

            var totalExtensionInPeriod = borrowingsInLastThreeMonths
                .Where(b => b.ReaderId == borrowing.ReaderId)
                .Sum(b => b.TotalExtensionDays);

            var maxExtensionInThreeMonths = reader.IsStaff
                ? maxExtensionDays * 2
                : maxExtensionDays;

            if (totalExtensionInPeriod + extensionDays > maxExtensionInThreeMonths)
            {
                throw new InvalidOperationException(
                    $"Cannot exceed {maxExtensionInThreeMonths} extension days in 3 months.");
            }

            // Perform extension
            borrowing.DueDate = borrowing.DueDate.AddDays(extensionDays);
            borrowing.TotalExtensionDays += extensionDays;
            borrowing.LastExtensionDate = extensionDate;

            try
            {
                this.borrowingRepository.Update(borrowing);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to extend borrowing record: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Extends a borrowing period with comprehensive validation (advanced version using current date).
        /// Validates: extension limit within last 3 months, book availability, and minimum percentage.
        /// </summary>
        public void ExtendBorrowingAdvanced(int borrowingId, int extensionDays)
        {
            this.ExtendBorrowing(borrowingId, extensionDays, DateTime.Now);
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

            var availablePercentage = (double)book.GetAvailableCopies() / book.TotalCopies;
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
                    .Count(b => b.Book != null && b.Book.Domains.Any(d => d.Id == domain.Id));

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

            var maxBooksPerDay = reader.IsStaff ? int.MaxValue : config.MaxBooksPerDay;
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