// <copyright file="ReaderServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for ReaderService without mocking.
    /// </summary>
    [TestClass]
    public class ReaderServiceTests
    {
        private ReaderService readerService;
        private ReaderDataService readerRepository;
        private LibraryDbContext context;

        /// <summary>
        /// Initializes test fixtures before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new LibraryDbContext();
            this.readerRepository = new ReaderDataService(this.context);
            this.readerService = new ReaderService(this.readerRepository);
        }

        /// <summary>
        /// Test 1: ValidateReader returns false for reader without email and phone.
        /// </summary>
        [TestMethod]
        public void ValidateReader_WithoutContactInfo_ReturnsFalse()
        {
            // Arrange
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                Email = null,
                PhoneNumber = null
            };

            // Act
            bool result = this.readerService.ValidateReader(reader);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test 2: CreateReader throws exception when reader is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateReader_WithNullReader_ThrowsException()
        {
            // Act
            this.readerService.CreateReader(null);
        }
    }
}