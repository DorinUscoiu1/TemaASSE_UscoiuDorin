// <copyright file="ReaderServiceTests.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace ServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using Service;
    using Data.Repositories;
    using Data;
    using FluentValidation;

    /// <summary>
    /// Unit tests for ReaderService with Rhino Mocks.
    /// </summary>
    [TestClass]
    public class ReaderServiceTests
    {
        private IReader mockReaderRepository;
        private ReaderService readerService;
        private LibraryConfiguration config;

        /// <summary>
        /// Initializes test fixtures before each test with mocked dependencies.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockReaderRepository = MockRepository.GenerateStub<IReader>();
            
            this.config = new LibraryConfiguration();

            this.readerService = new ReaderService(this.mockReaderRepository);
        }

        /// <summary>
        /// Test 1: GetAllReaders returns all readers from repository.
        /// </summary>
        [TestMethod]
        public void GetAllReaders_WhenCalled_ReturnsAllReaders()
        {
            // Arrange
            var readers = new List<Reader>
            {
                new Reader { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Address = "123 Main St" },
                new Reader { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Address = "456 Oak Ave" }
            };

            this.mockReaderRepository.Stub(x => x.GetAll()).Return(readers);

            // Act
            var result = this.readerService.GetAllReaders();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// Test 2: GetReaderById returns correct reader.
        /// </summary>
        [TestMethod]
        public void GetReaderById_WithValidId_ReturnsCorrectReader()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Address = "123 Main St"
            };

            this.mockReaderRepository.Stub(x => x.GetById(1)).Return(reader);

            // Act
            var result = this.readerService.GetReaderById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.FirstName);
            Assert.AreEqual("john@example.com", result.Email);
        }

        /// <summary>
        /// Test 3: CreateReader throws ValidationException when email invalid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateReader_WithInvalidEmail_ThrowsException()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "invalid-email",  // ? Invalid email format
                Address = "123 Main St",
                PhoneNumber = "555-1234"  // ? Valid contact
            };

            // Act
            this.readerService.CreateReader(reader);
        }

        /// <summary>
        /// Test 4: CreateReader throws ValidationException when no contact info.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateReader_WithoutContactInfo_ThrowsException()
        {
            // Arrange
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Doe",
                Email = string.Empty,
                PhoneNumber = string.Empty,
                Address = "123 Main St"
            };

            // Act
            this.readerService.CreateReader(reader);
        }

        /// <summary>
        /// Test 5: CreateReader with valid data calls repository.
        /// </summary>
        [TestMethod]
        public void CreateReader_WithValidData_CallsRepositoryAdd()
        {
            // Arrange
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Address = "123 Main St",
                PhoneNumber = "555-1234"
            };

            // Act
            this.readerService.CreateReader(reader);

            // Assert
            this.mockReaderRepository.AssertWasCalled(x => x.Add(Arg<Reader>.Is.Anything));
        }

        /// <summary>
        /// Test 6: UpdateReader successfully updates reader information.
        /// </summary>
        [TestMethod]
        public void UpdateReader_WithValidData_CallsRepositoryUpdate()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Address = "123 Main St",
                PhoneNumber = "555-1234"
            };

            // Act
            this.readerService.UpdateReader(reader);

            // Assert
            this.mockReaderRepository.AssertWasCalled(x => x.Update(reader));
        }

        /// <summary>
        /// Test 7: GetStaffMembers returns only staff readers.
        /// </summary>
        [TestMethod]
        public void GetStaffMembers_WhenCalled_ReturnsOnlyStaff()
        {
            // Arrange
            var staffMembers = new List<Reader>
            {
                new Reader { Id = 1, FirstName = "Jane", LastName = "Staff", IsStaff = true, Email = "jane@example.com", Address = "123 Main St" }
            };

            this.mockReaderRepository.Stub(x => x.GetStaffMembers()).Return(staffMembers);

            // Act
            var result = this.readerService.GetStaffMembers();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(r => r.IsStaff));
        }

        /// <summary>
        /// Test 8: GetRegularReaders returns only non-staff readers.
        /// </summary>
        [TestMethod]
        public void GetRegularReaders_WhenCalled_ReturnsOnlyRegular()
        {
            // Arrange
            var regularReaders = new List<Reader>
            {
                new Reader { Id = 1, FirstName = "John", LastName = "Doe", IsStaff = false, Email = "john@example.com", Address = "123 Main St" }
            };

            this.mockReaderRepository.Stub(x => x.GetRegularReaders()).Return(regularReaders);

            // Act
            var result = this.readerService.GetRegularReaders();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(r => !r.IsStaff));
        }

        #region Database Tests

        /// <summary>
        /// Test 9: Insert reader into database and verify it was stored.
        /// This is an integration test with real database.
        /// </summary>
        [TestMethod]
        public void InsertReader_IntoDatabase_SuccessfullyStored()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new ReaderDataService(context);
                var realService = new ReaderService(realRepository);

                var reader = new Reader
                {
                    FirstName = "TestDB",
                    LastName = "User",
                    Email = "testdb-user@example.com",
                    Address = "999 Database Street",
                    PhoneNumber = "555-9999",
                    IsStaff = false
                };

                // Act
                realService.CreateReader(reader);

                // Assert
                var retrievedReader = context.Readers
                    .FirstOrDefault(r => r.Email == "testdb-user@example.com");

                Assert.IsNotNull(retrievedReader, "Reader should exist in database");
                Assert.AreEqual("TestDB", retrievedReader.FirstName);
                Assert.AreEqual("User", retrievedReader.LastName);
                Assert.AreEqual("999 Database Street", retrievedReader.Address);
                Assert.AreEqual("555-9999", retrievedReader.PhoneNumber);
                Assert.IsTrue(retrievedReader.Id > 0, "Reader should have an ID");

                // Cleanup
                context.Readers.Remove(retrievedReader);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 10: Retrieve inserted reader from database by ID.
        /// </summary>
        [TestMethod]
        public void GetReaderById_FromDatabase_ReturnsCorrectData()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new ReaderDataService(context);
                var realService = new ReaderService(realRepository);

                var reader = new Reader
                {
                    FirstName = "DatabaseTest",
                    LastName = "Reader",
                    Email = "dbtest-reader@example.com",
                    Address = "888 Test Avenue",
                    PhoneNumber = "555-8888",
                    IsStaff = false
                };

                realService.CreateReader(reader);

                var insertedReader = context.Readers
                    .FirstOrDefault(r => r.Email == "dbtest-reader@example.com");
                Assert.IsNotNull(insertedReader);
                int readerId = insertedReader.Id;

                // Act
                var retrievedReader = realService.GetReaderById(readerId);

                // Assert
                Assert.IsNotNull(retrievedReader);
                Assert.AreEqual("DatabaseTest", retrievedReader.FirstName);
                Assert.AreEqual("dbtest-reader@example.com", retrievedReader.Email);

                // Cleanup
                context.Readers.Remove(insertedReader);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 11: Update reader in database and verify changes are persisted.
        /// </summary>
        [TestMethod]
        public void UpdateReader_InDatabase_ChangesPersisted()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new ReaderDataService(context);
                var realService = new ReaderService(realRepository);

                var reader = new Reader
                {
                    FirstName = "OriginalName",
                    LastName = "OriginalLast",
                    Email = "original@example.com",
                    Address = "777 Original Street",
                    PhoneNumber = "555-7777"
                };

                realService.CreateReader(reader);

                var insertedReader = context.Readers
                    .FirstOrDefault(r => r.Email == "original@example.com");
                Assert.IsNotNull(insertedReader);

                // Modify reader
                insertedReader.FirstName = "UpdatedName";
                insertedReader.Address = "888 Updated Street";

                // Act
                realService.UpdateReader(insertedReader);

                // Assert
                var updatedReader = context.Readers.Find(insertedReader.Id);
                Assert.AreEqual("UpdatedName", updatedReader.FirstName);
                Assert.AreEqual("888 Updated Street", updatedReader.Address);

                // Cleanup
                context.Readers.Remove(updatedReader);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 12: Delete reader from database and verify it is removed.
        /// </summary>
        [TestMethod]
        public void DeleteReader_FromDatabase_IsRemoved()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new ReaderDataService(context);
                var realService = new ReaderService(realRepository);

                var reader = new Reader
                {
                    FirstName = "ToDelete",
                    LastName = "User",
                    Email = "todelete@example.com",
                    Address = "666 Delete Lane",
                    PhoneNumber = "555-6666"
                };

                realService.CreateReader(reader);

                var insertedReader = context.Readers
                    .FirstOrDefault(r => r.Email == "todelete@example.com");
                Assert.IsNotNull(insertedReader);
                int readerId = insertedReader.Id;

                // Act
                realService.DeleteReader(readerId);

                // Assert
                var deletedReader = context.Readers.Find(readerId);
                Assert.IsNull(deletedReader, "Reader should be deleted from database");
            }
        }

        #endregion
    }
}