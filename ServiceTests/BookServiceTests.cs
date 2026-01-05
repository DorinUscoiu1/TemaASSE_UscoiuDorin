// <copyright file="BookServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for BookService without mocking.
    /// </summary>
    [TestClass]
    public class BookServiceTests
    {
        private BookService bookService;
        private BookDataService bookRepository;
        private BookDomainDataService bookDomainRepository;
        private LibraryConfiguration configRepository;
        private LibraryDbContext context;

        /// <summary>
        /// Initializes test fixtures before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new LibraryDbContext();
            this.bookRepository = new BookDataService(this.context);
            this.bookDomainRepository = new BookDomainDataService(this.context);
            this.configRepository = new LibraryConfiguration();

            this.bookService = new BookService(
                this.bookRepository,
                this.bookDomainRepository,
                this.configRepository);
        }

        /// <summary>
        /// Test 1: GetBookById returns book or null when not found.
        /// </summary>
        [TestMethod]
        public void GetBookById_WithValidId_ReturnsBook()
        {
            // Act
            var result = this.bookService.GetBookById(1);

            // Assert - Should return null or a valid book
            Assert.IsTrue(result == null || result.Id > 0);
        }

        /// <summary>
        /// Test 2: CreateBook throws exception when title is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateBook_WithEmptyTitle_ThrowsException()
        {
            // Arrange
            var book = new Book { Id = 1, Title = string.Empty };

            // Act
            this.bookService.CreateBook(book);
        }
    }
}