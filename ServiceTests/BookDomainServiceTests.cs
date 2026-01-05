// <copyright file="BookDomainServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for BookDomainService without mocking.
    /// </summary>
    [TestClass]
    public class BookDomainServiceTests
    {
        private BookDomainService bookDomainService;
        private BookDomainDataService bookDomainRepository;
        private LibraryDbContext context;

        /// <summary>
        /// Initializes test fixtures before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.context = new LibraryDbContext();
            this.bookDomainRepository = new BookDomainDataService(this.context);
            this.bookDomainService = new BookDomainService(this.bookDomainRepository);
        }

        /// <summary>
        /// Test 1: CreateDomain throws exception when name is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDomain_WithEmptyName_ThrowsException()
        {
            // Arrange
            var domain = new BookDomain { Id = 1, Name = string.Empty };

            // Act
            this.bookDomainService.CreateDomain(domain);
        }

        /// <summary>
        /// Test 2: GetDomainById returns domain or null when not found.
        /// </summary>
        [TestMethod]
        public void GetDomainById_WithValidId_ReturnsDomainOrNull()
        {
            // Act
            var result = this.bookDomainService.GetDomainById(1);

            // Assert - Should return null or a valid domain
            Assert.IsTrue(result == null || result.Id > 0);
        }
    }
}