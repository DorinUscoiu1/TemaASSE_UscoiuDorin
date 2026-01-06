// <copyright file="AuthorServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for AuthorService with comprehensive coverage.
    /// </summary>
    [TestClass]
    public class AuthorServiceTests
    {
        private IAuthor mockAuthorRepository;
        private AuthorService authorService;
        private LibraryConfiguration config;

        /// <summary>
        /// Initializes test fixtures before each test with mocked dependencies.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockAuthorRepository = MockRepository.GenerateStub<IAuthor>();
            this.config = new LibraryConfiguration();
            this.authorService = new AuthorService(this.mockAuthorRepository, this.config);
        }

        /// <summary>
        /// Test 1: GetAllAuthors returns all authors from repository.
        /// </summary>
        [TestMethod]
        public void GetAllAuthors_WhenCalled_ReturnsAllAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { Id = 1, FirstName = "Isaac", LastName = "Newton" },
                new Author { Id = 2, FirstName = "Albert", LastName = "Einstein" },
                new Author { Id = 3, FirstName = "Marie", LastName = "Curie" }
            };

            this.mockAuthorRepository.Stub(x => x.GetAll()).Return(authors);

            // Act
            var result = this.authorService.GetAllAuthors();

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(a => a.FirstName == "Isaac"));
        }

        /// <summary>
        /// Test 2: GetAuthorById returns correct author when found.
        /// </summary>
        [TestMethod]
        public void GetAuthorById_WithValidId_ReturnsCorrectAuthor()
        {
            // Arrange
            var author = new Author { Id = 1, FirstName = "Stephen", LastName = "Hawking" };
            this.mockAuthorRepository.Stub(x => x.GetById(1)).Return(author);

            // Act
            var result = this.authorService.GetAuthorById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Stephen", result.FirstName);
            Assert.AreEqual("Hawking", result.LastName);
        }

        /// <summary>
        /// Test 3: GetAuthorById returns null when author not found.
        /// </summary>
        [TestMethod]
        public void GetAuthorById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            this.mockAuthorRepository.Stub(x => x.GetById(999)).Return(null);

            // Act
            var result = this.authorService.GetAuthorById(999);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Test 4: CreateAuthor throws exception when author is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateAuthor_WithNullAuthor_ThrowsArgumentNullException()
        {
            // Act
            this.authorService.CreateAuthor(null);
        }

        /// <summary>
        /// Test 5: CreateAuthor with valid data calls repository.
        /// </summary>
        [TestMethod]
        public void CreateAuthor_WithValidData_CallsRepositoryAdd()
        {
            // Arrange
            var author = new Author
            {
                FirstName = "Carl",
                LastName = "Sagan"
            };

            // Act
            this.authorService.CreateAuthor(author);

            // Assert
            this.mockAuthorRepository.AssertWasCalled(x => x.Add(Arg<Author>.Is.Anything));
        }

        /// <summary>
        /// Test 6: UpdateAuthor successfully updates author information.
        /// </summary>
        [TestMethod]
        public void UpdateAuthor_WithValidData_CallsRepositoryUpdate()
        {
            // Arrange
            var author = new Author
            {
                Id = 1,
                FirstName = "Alan",
                LastName = "Turing"
            };

            // Act
            this.authorService.UpdateAuthor(author);

            // Assert
            this.mockAuthorRepository.AssertWasCalled(x => x.Update(author));
        }

        /// <summary>
        /// Test 7: DeleteAuthor calls repository delete method.
        /// </summary>
        [TestMethod]
        public void DeleteAuthor_WithValidId_CallsRepositoryDelete()
        {
            // Act
            this.authorService.DeleteAuthor(1);

            // Assert
            this.mockAuthorRepository.AssertWasCalled(x => x.Delete(1));
        }

        /// <summary>
        /// Test 8: GetBooksByAuthor returns books written by an author.
        /// </summary>
        [TestMethod]
        public void GetBooksByAuthor_WithValidAuthorId_ReturnsAuthorBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book One" },
                new Book { Id = 2, Title = "Book Two" }
            };

            var author = new Author
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Books = books
            };

            this.mockAuthorRepository.Stub(x => x.GetById(1)).Return(author);

            // Act
            var result = this.authorService.GetBooksByAuthor(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(b => b.Title == "Book One"));
        }

        /// <summary>
        /// Test 9: GetBooksByAuthor returns empty when author not found.
        /// </summary>
        [TestMethod]
        public void GetBooksByAuthor_WithInvalidAuthorId_ReturnsEmpty()
        {
            // Arrange
            this.mockAuthorRepository.Stub(x => x.GetById(999)).Return(null);

            // Act
            var result = this.authorService.GetBooksByAuthor(999);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        /// <summary>
        /// Test 10: GetAuthorsByFirstName returns authors with matching first name.
        /// </summary>
        [TestMethod]
        public void GetAuthorsByFirstName_WithValidName_ReturnsMatchingAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { Id = 1, FirstName = "John", LastName = "Doe" },
                new Author { Id = 2, FirstName = "John", LastName = "Smith" }
            };

            this.mockAuthorRepository.Stub(x => x.GetByFirstName("John")).Return(authors);

            // Act
            var result = this.authorService.GetAuthorsByFirstName("John");

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(a => a.FirstName == "John"));
        }

        /// <summary>
        /// Test 11: GetAuthorsByFirstName with empty string returns empty.
        /// </summary>
        [TestMethod]
        public void GetAuthorsByFirstName_WithEmptyString_ReturnsEmpty()
        {
            // Act
            var result = this.authorService.GetAuthorsByFirstName(string.Empty);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        /// <summary>
        /// Test 12: GetAuthorsByLastName returns authors with matching last name.
        /// </summary>
        [TestMethod]
        public void GetAuthorsByLastName_WithValidName_ReturnsMatchingAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { Id = 1, FirstName = "John", LastName = "Doe" },
                new Author { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };

            this.mockAuthorRepository.Stub(x => x.GetByLastName("Doe")).Return(authors);

            // Act
            var result = this.authorService.GetAuthorsByLastName("Doe");

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(a => a.LastName == "Doe"));
        }

        /// <summary>
        /// Test 13: Database integration - insert author into database.
        /// </summary>
        [TestMethod]
        public void InsertAuthor_IntoDatabase_SuccessfullyStored()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new AuthorDataService(context);
                var realService = new AuthorService(realRepository, this.config);

                var author = new Author
                {
                    FirstName = "Donald",
                    LastName = "Knuth"
                };

                // Act
                realService.CreateAuthor(author);

                // Assert
                var retrievedAuthor = context.Authors
                    .FirstOrDefault(a => a.FirstName == "Donald" && a.LastName == "Knuth");

                Assert.IsNotNull(retrievedAuthor, "Author should exist in database");
                Assert.AreEqual("Donald", retrievedAuthor.FirstName);
                Assert.IsTrue(retrievedAuthor.Id > 0, "Author should have an ID");

                // Cleanup
                context.Authors.Remove(retrievedAuthor);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 14: Database integration - update author in database.
        /// </summary>
        [TestMethod]
        public void UpdateAuthor_InDatabase_ChangesPersisted()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new AuthorDataService(context);
                var realService = new AuthorService(realRepository, this.config);

                var author = new Author
                {
                    FirstName = "Richard",
                    LastName = "Feynman"
                };

                realService.CreateAuthor(author);

                var insertedAuthor = context.Authors
                    .FirstOrDefault(a => a.FirstName == "Richard" && a.LastName == "Feynman");
                Assert.IsNotNull(insertedAuthor);

                // Modify author
                insertedAuthor.FirstName = "Richard P.";

                // Act
                realService.UpdateAuthor(insertedAuthor);

                // Assert
                var updatedAuthor = context.Authors.Find(insertedAuthor.Id);
                Assert.AreEqual("Richard P.", updatedAuthor.FirstName);

                // Cleanup
                context.Authors.Remove(updatedAuthor);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 15: Database integration - delete author from database.
        /// </summary>
        [TestMethod]
        public void DeleteAuthor_FromDatabase_IsRemoved()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new AuthorDataService(context);
                var realService = new AuthorService(realRepository, this.config);

                var author = new Author
                {
                    FirstName = "Blaise",
                    LastName = "Pascal"
                };

                realService.CreateAuthor(author);

                var insertedAuthor = context.Authors
                    .FirstOrDefault(a => a.FirstName == "Blaise" && a.LastName == "Pascal");
                Assert.IsNotNull(insertedAuthor);
                int authorId = insertedAuthor.Id;

                // Act
                realService.DeleteAuthor(authorId);

                // Assert
                var deletedAuthor = context.Authors.Find(authorId);
                Assert.IsNull(deletedAuthor, "Author should be deleted from database");
            }
        }
    }
}