// <copyright file="BookDomainServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for BookDomainService with Rhino Mocks and Integration Tests.
    /// </summary>
    [TestClass]
    public class BookDomainServiceTests
    {
        private IBookDomain mockDomainRepository;
        private LibraryConfiguration mockConfigRepository;
        private BookDomainService domainService;

        /// <summary>
        /// Initializes test fixtures before each test with mocked dependencies.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockDomainRepository = MockRepository.GenerateStub<IBookDomain>();
            this.mockConfigRepository = new LibraryConfiguration();

            this.domainService = new BookDomainService(
                this.mockDomainRepository);
        }

        #region Unit Tests with Mocks

        /// <summary>
        /// Test 1: GetAllDomains returns all domains from repository.
        /// </summary>
        [TestMethod]
        public void GetAllDomains_WhenCalled_ReturnsAllDomains()
        {
            // Arrange
            var domains = new List<BookDomain>
            {
                new BookDomain { Id = 1, Name = "Science" },
                new BookDomain { Id = 2, Name = "Technology" },
                new BookDomain { Id = 3, Name = "Mathematics" }
            };

            this.mockDomainRepository.Stub(x => x.GetAll()).Return(domains);

            // Act
            var result = this.domainService.GetAllDomains();

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(d => d.Name == "Science"));
        }

        /// <summary>
        /// Test 2: GetDomainById returns correct domain.
        /// </summary>
        [TestMethod]
        public void GetDomainById_WithValidId_ReturnsCorrectDomain()
        {
            // Arrange
            var domain = new BookDomain { Id = 1, Name = "Science", ParentDomainId = null };
            this.mockDomainRepository.Stub(x => x.GetById(1)).Return(domain);

            // Act
            var result = this.domainService.GetDomainById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Science", result.Name);
        }

        /// <summary>
        /// Test 3: GetRootDomains returns only root domains.
        /// </summary>
        [TestMethod]
        public void GetRootDomains_WhenCalled_ReturnsOnlyRootDomains()
        {
            // Arrange
            var rootDomains = new List<BookDomain>
            {
                new BookDomain { Id = 1, Name = "Science", ParentDomainId = null },
                new BookDomain { Id = 2, Name = "Art", ParentDomainId = null }
            };

            this.mockDomainRepository.Stub(x => x.GetRootDomains()).Return(rootDomains);

            // Act
            var result = this.domainService.GetRootDomains();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(d => d.ParentDomainId == null));
        }

        /// <summary>
        /// Test 4: GetSubdomains returns subdomains of a domain.
        /// </summary>
        [TestMethod]
        public void GetSubdomains_WithValidParentId_ReturnsSubdomains()
        {
            // Arrange
            var subdomains = new List<BookDomain>
            {
                new BookDomain { Id = 2, Name = "Physics", ParentDomainId = 1 },
                new BookDomain { Id = 3, Name = "Chemistry", ParentDomainId = 1 }
            };

            this.mockDomainRepository.Stub(x => x.GetSubdomains(1)).Return(subdomains);

            // Act
            var result = this.domainService.GetSubdomains(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(d => d.ParentDomainId == 1));
        }

        /// <summary>
        /// Test 5: CreateDomain throws exception when name is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDomain_WithNullName_ThrowsArgumentException()
        {
            // Arrange
            var domain = new BookDomain { Name = null };

            // Act
            this.domainService.CreateDomain(domain);
        }

        /// <summary>
        /// Test 6: CreateDomain throws exception when name is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateDomain_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var domain = new BookDomain { Name = string.Empty };

            // Act
            this.domainService.CreateDomain(domain);
        }

        /// <summary>
        /// Test 7: CreateDomain with valid data succeeds.
        /// </summary>
        [TestMethod]
        public void CreateDomain_WithValidData_SuccessfullyCreates()
        {
            // Arrange
            var domain = new BookDomain { Name = "Science" };

            // Act
            this.domainService.CreateDomain(domain);

            // Assert
            this.mockDomainRepository.AssertWasCalled(x => x.Add(Arg<BookDomain>.Is.Anything));
        }

        #endregion

        #region Database Integration Tests

        /// <summary>
        /// Test 8: CreateDomain inserts domain into database successfully.
        /// </summary>
        [TestMethod]
        public void CreateDomain_IntoDatabase_SuccessfullyStored()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDomainDataService(context);
                var realService = new BookDomainService(realRepository);

                var domain = new BookDomain
                {
                    Name = "Integration Test Domain",
                    ParentDomainId = null
                };

                // Act
                realService.CreateDomain(domain);

                // Assert
                var retrievedDomain = context.Domains
                    .FirstOrDefault(d => d.Name == "Integration Test Domain");

                Assert.IsNotNull(retrievedDomain, "Domain should exist in database");
                Assert.AreEqual("Integration Test Domain", retrievedDomain.Name);
                Assert.IsNull(retrievedDomain.ParentDomainId);
                Assert.IsTrue(retrievedDomain.Id > 0);

                // Cleanup
                context.Domains.Remove(retrievedDomain);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 9: CreateDomain with subdomain stores parent-child relationship.
        /// </summary>
        [TestMethod]
        public void CreateDomain_WithParentDomain_StoresParentChildRelationship()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDomainDataService(context);
                var realService = new BookDomainService(realRepository);

                // First create parent domain
                var parentDomain = new BookDomain { Name = "Parent Science Domain", ParentDomainId = null };
                realService.CreateDomain(parentDomain);

                var createdParent = context.Domains
                    .FirstOrDefault(d => d.Name == "Parent Science Domain");
                Assert.IsNotNull(createdParent);

                // Create subdomain
                var childDomain = new BookDomain
                {
                    Name = "Child Physics Domain",
                    ParentDomainId = createdParent.Id
                };

                // Act
                realService.CreateDomain(childDomain);

                // Assert
                var retrievedChild = context.Domains
                    .FirstOrDefault(d => d.Name == "Child Physics Domain");

                Assert.IsNotNull(retrievedChild);
                Assert.AreEqual(createdParent.Id, retrievedChild.ParentDomainId);

                // Cleanup
                context.Domains.Remove(retrievedChild);
                context.Domains.Remove(createdParent);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 10: GetDomainById from database returns correct domain.
        /// </summary>
        [TestMethod]
        public void GetDomainById_FromDatabase_ReturnsCorrectDomain()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDomainDataService(context);
                var realService = new BookDomainService(realRepository);

                var domain = new BookDomain { Name = "GetById Test Domain" };
                realService.CreateDomain(domain);

                var insertedDomain = context.Domains
                    .FirstOrDefault(d => d.Name == "GetById Test Domain");
                Assert.IsNotNull(insertedDomain);
                int domainId = insertedDomain.Id;

                // Act
                var retrievedDomain = realService.GetDomainById(domainId);

                // Assert
                Assert.IsNotNull(retrievedDomain);
                Assert.AreEqual(domainId, retrievedDomain.Id);
                Assert.AreEqual("GetById Test Domain", retrievedDomain.Name);

                // Cleanup
                context.Domains.Remove(insertedDomain);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 11: UpdateDomain persists changes to database.
        /// </summary>
        [TestMethod]
        public void UpdateDomain_InDatabase_PersistsChanges()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDomainDataService(context);
                var realService = new BookDomainService(realRepository);

                var domain = new BookDomain { Name = "Original Domain Name" };
                realService.CreateDomain(domain);

                var insertedDomain = context.Domains
                    .FirstOrDefault(d => d.Name == "Original Domain Name");
                Assert.IsNotNull(insertedDomain);

                // Modify domain
                insertedDomain.Name = "Updated Domain Name";

                // Act
                realService.UpdateDomain(insertedDomain);

                // Assert
                var updatedDomain = context.Domains.Find(insertedDomain.Id);
                Assert.AreEqual("Updated Domain Name", updatedDomain.Name);

                // Cleanup
                context.Domains.Remove(updatedDomain);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 12: DeleteDomain removes domain from database.
        /// </summary>
        [TestMethod]
        public void DeleteDomain_FromDatabase_IsRemoved()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDomainDataService(context);
                var realService = new BookDomainService(realRepository);

                var domain = new BookDomain { Name = "Delete Test Domain" };
                realService.CreateDomain(domain);

                var insertedDomain = context.Domains
                    .FirstOrDefault(d => d.Name == "Delete Test Domain");
                Assert.IsNotNull(insertedDomain);
                int domainId = insertedDomain.Id;

                // Act
                realService.DeleteDomain(domainId);

                // Assert
                var deletedDomain = context.Domains.Find(domainId);
                Assert.IsNull(deletedDomain, "Domain should be deleted from database");
            }
        }

        #endregion
    }
}