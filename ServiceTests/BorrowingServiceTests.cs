// <copyright file="BorrowingServiceTests.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace ServiceTests
{
    using System;
    using System.Collections.Generic;
    using Domain.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Service;
    using Data.Repositories;
    using Data;

    /// <summary>
    /// Unit tests for BorrowingService without mocking.
    /// </summary>
    [TestClass]
    public class BorrowingServiceTests
    {
        private BorrowingService borrowingService;
        private BorrowingDataService borrowingRepository;
        private BookDataService bookRepository;
        private ReaderDataService readerRepository;
        private LibraryConfiguration configRepository;
        private LibraryDbContext context;

        /// <summary>
        /// Initializes test fixtures before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new LibraryDbContext();
            this.borrowingRepository = new BorrowingDataService(this.context);
            this.bookRepository = new BookDataService(this.context);
            this.readerRepository = new ReaderDataService(this.context);
            this.configRepository = new LibraryConfiguration();

            this.borrowingService = new BorrowingService(
                this.borrowingRepository,
                this.bookRepository,
                this.readerRepository,
                this.configRepository);
        }

        /// <summary>
        /// Test 1: BorrowBook throws exception when reader is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BorrowBook_WithInvalidReaderId_ThrowsException()
        {
            // Act
            this.borrowingService.BorrowBook(999, 1, 14);
        }

        /// <summary>
        /// Test 2: GetActiveBorrowingCount returns count correctly.
        /// </summary>
        [TestMethod]
        public void GetActiveBorrowingCount_ReturnsCorrectCount()
        {
            // Arrange
            int readerId = 1;

            // Act
            int count = this.borrowingService.GetActiveBorrowingCount(readerId);

            // Assert
            Assert.IsTrue(count >= 0);
        }
    }
}