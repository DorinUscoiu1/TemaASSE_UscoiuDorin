// <copyright file="BorrowingServiceTests.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace ServiceTests
{
    using Data;
    using Data.Repositories;
    using Domain.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Unit tests for BorrowingService with Rhino Mocks.
    /// </summary>
    [TestClass]
    public class BorrowingServiceTests
    {
        private IBorrowing mockBorrowingRepository;
        private IBook mockBookRepository;
        private IReader mockReaderRepository;
        private LibraryConfiguration mockConfigRepository;
        private BorrowingService borrowingService;

        /// <summary>
        /// Initializes test fixtures before each test with mocked dependencies.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockBorrowingRepository = MockRepository.GenerateStub<IBorrowing>();
            this.mockBookRepository = MockRepository.GenerateStub<IBook>();
            this.mockReaderRepository = MockRepository.GenerateStub<IReader>();
            this.mockConfigRepository = new LibraryConfiguration();

            this.borrowingService = new BorrowingService(
                this.mockBorrowingRepository,
                this.mockBookRepository,
                this.mockReaderRepository,
                this.mockConfigRepository);
        }

        /// <summary>
        /// Test 1: BorrowBook throws exception when reader not found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BorrowBook_WithInvalidReader_ThrowsException()
        {
            // Arrange
            this.mockReaderRepository.Stub(x => x.GetById(999)).Return(null);
            this.mockBookRepository.Stub(x => x.GetById(1))
                .Return(new Book { Id = 1, Title = "Test Book" });

            // Act
            this.borrowingService.BorrowBook(999, 1, 14);
        }

        /// <summary>
        /// Test 2: BorrowBook throws exception when book not found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BorrowBook_WithInvalidBook_ThrowsException()
        {
            // Arrange
            this.mockReaderRepository.Stub(x => x.GetById(1))
                .Return(new Reader { Id = 1, FirstName = "John", LastName = "Doe" });
            this.mockBookRepository.Stub(x => x.GetById(999)).Return(null);

            // Act
            this.borrowingService.BorrowBook(1, 999, 14);
        }

        /// <summary>
        /// Test 3: GetActiveBorrowings returns active borrowings for reader.
        /// </summary>
        [TestMethod]
        public void GetActiveBorrowings_WithValidReaderId_ReturnsActiveBorrowings()
        {
            // Arrange
            var activeBorrowings = new List<Borrowing>
            {
                new Borrowing { Id = 1, ReaderId = 1, IsActive = true },
                new Borrowing { Id = 2, ReaderId = 1, IsActive = true }
            };

            this.mockBorrowingRepository.Stub(x => x.GetActiveBorrowingsByReader(1))
                .Return(activeBorrowings);

            // Act
            var result = this.borrowingService.GetActiveBorrowings(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(b => b.IsActive));
        }

        /// <summary>
        /// Test 4: GetOverdueBorrowings returns overdue records.
        /// </summary>
        [TestMethod]
        public void GetOverdueBorrowings_WhenCalled_ReturnsOverdueRecords()
        {
            // Arrange
            var overdueBorrowings = new List<Borrowing>
            {
                new Borrowing
                {
                    Id = 1,
                    DueDate = DateTime.Now.AddDays(-5),
                    IsActive = true
                },
                new Borrowing
                {
                    Id = 2,
                    DueDate = DateTime.Now.AddDays(-3),
                    IsActive = true
                }
            };

            this.mockBorrowingRepository.Stub(x => x.GetOverdueBorrowings())
                .Return(overdueBorrowings);

            // Act
            var result = this.borrowingService.GetOverdueBorrowings();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// Test 5: ExtendBorrowing throws exception when exceeding max extension days.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtendBorrowing_WithExceededExtensionDays_ThrowsException()
        {
            // Arrange
            var borrowing = new Borrowing
            {
                Id = 1,
                DueDate = DateTime.Now.AddDays(5),
                TotalExtensionDays = 5
            };

            this.borrowingService.ExtendBorrowing(1, 5);
        }

        /// <summary>
        /// Test 6: CanBorrowBook returns false when no available copies.
        /// </summary>
        [TestMethod]
        public void CanBorrowBook_WithNoAvailableCopies_ReturnsFalse()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                IsStaff = false
            };

            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = 5,
                BorrowingRecords = new List<Borrowing>()
            };

            this.mockReaderRepository.Stub(x => x.GetById(1)).Return(reader);
            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);

            // Act
            var result = this.borrowingService.CanBorrowBook(1, 1);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test 7: GetActiveBorrowingCount returns correct count.
        /// </summary>
        [TestMethod]
        public void GetActiveBorrowingCount_WithValidReaderId_ReturnsCorrectCount()
        {
            // Arrange
            var activeBorrowings = new List<Borrowing>
            {
                new Borrowing { Id = 1, ReaderId = 1, IsActive = true },
                new Borrowing { Id = 2, ReaderId = 1, IsActive = true },
                new Borrowing { Id = 3, ReaderId = 1, IsActive = true }
            };

            this.mockBorrowingRepository.Stub(x => x.GetActiveBorrowingsByReader(1))
                .Return(activeBorrowings);

            // Act
            var result = this.borrowingService.GetActiveBorrowingCount(1);

            // Assert
            Assert.AreEqual(3, result);
        }

        /// <summary>
        /// Test 8: CanBorrowBook returns true when all conditions met.
        /// </summary>
        [TestMethod]
        public void CanBorrowBook_WithAllConditionsMet_ReturnsTrue()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                IsStaff = false
            };

            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                TotalCopies = 10,
                ReadingRoomOnlyCopies = 2,
                BorrowingRecords = new List<Borrowing>(),
                Domains = new List<BookDomain>()
            };

            this.mockReaderRepository.Stub(x => x.GetById(1)).Return(reader);
            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);
            this.mockBorrowingRepository.Stub(x => x.GetActiveBorrowingsByReader(1))
                .Return(new List<Borrowing>());
            this.mockBorrowingRepository.Stub(x => x.GetBorrowingsByDateRange(
                Arg<DateTime>.Is.Anything, Arg<DateTime>.Is.Anything))
                .Return(new List<Borrowing>());
            this.mockBorrowingRepository.Stub(x => x.GetBorrowingsByBook(1))
                .Return(new List<Borrowing>());

            // Act
            var result = this.borrowingService.CanBorrowBook(1, 1);

            // Assert
            Assert.IsTrue(result);
        }
        
    }
    
}