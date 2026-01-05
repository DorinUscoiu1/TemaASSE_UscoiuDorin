// <copyright file="EditionServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for EditionService without mocking.
    /// </summary>
    [TestClass]
    public class EditionServiceTests
    {
        private EditionService editionService;
        private EditionDataService editionRepository;
        private BookDataService bookRepository;
        private LibraryDbContext context;

        /// <summary>
        /// Initializes test fixtures before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new LibraryDbContext();
            this.editionRepository = new EditionDataService(this.context);
            this.bookRepository = new BookDataService(this.context);

            this.editionService = new EditionService(
                this.editionRepository,
                this.bookRepository);
        }

        /// <summary>
        /// Test 1: CreateEdition throws exception when year is invalid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEdition_WithInvalidYear_ThrowsException()
        {
            // Arrange
            var edition = new Edition
            {
                Id = 1,
                BookId = 1,
                Publisher = "Publisher",
                Year = 0,
                PageCount = 300,
                EditionNumber = 1
            };

            // Act
            this.editionService.CreateEdition(edition);
        }

        /// <summary>
        /// Test 2: CreateEdition throws exception when publisher is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateEdition_WithEmptyPublisher_ThrowsException()
        {
            // Arrange
            var edition = new Edition
            {
                Id = 1,
                BookId = 1,
                Publisher = string.Empty,
                Year = 2023,
                PageCount = 300,
                EditionNumber = 1
            };

            // Act
            this.editionService.CreateEdition(edition);
        }
    }
}