// <copyright file="EditionServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for EditionService with comprehensive coverage.
    /// </summary>
    [TestClass]
    public class EditionServiceTests
    {
        private IEdition mockEditionRepository;
        private IBook mockBookRepository;
        private EditionService editionService;

        /// <summary>
        /// Initializes test fixtures before each test with mocked dependencies.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockEditionRepository = MockRepository.GenerateStub<IEdition>();
            this.mockBookRepository = MockRepository.GenerateStub<IBook>();

            this.editionService = new EditionService(
                this.mockEditionRepository,
                this.mockBookRepository);
        }

        /// <summary>
        /// Test 1: GetAllEditions returns all editions from repository.
        /// </summary>
        [TestMethod]
        public void GetAllEditions_WhenCalled_ReturnsAllEditions()
        {
            // Arrange
            var editions = new List<Edition>
            {
                new Edition { Id = 1, BookId = 1, Publisher = "O'Reilly", Year = 2020 },
                new Edition { Id = 2, BookId = 2, Publisher = "Microsoft Press", Year = 2021 },
                new Edition { Id = 3, BookId = 3, Publisher = "Packt", Year = 2022 }
            };

            this.mockEditionRepository.Stub(x => x.GetAll()).Return(editions);

            // Act
            var result = this.editionService.GetAllEditions();

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        /// <summary>
        /// Test 2: GetEditionById returns correct edition when found.
        /// </summary>
        [TestMethod]
        public void GetEditionById_WithValidId_ReturnsCorrectEdition()
        {
            // Arrange
            var edition = new Edition
            {
                Id = 1,
                BookId = 1,
                Publisher = "Addison-Wesley",
                Year = 2019,
                EditionNumber = 1,
                PageCount = 500
            };

            this.mockEditionRepository.Stub(x => x.GetById(1)).Return(edition);

            // Act
            var result = this.editionService.GetEditionById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Addison-Wesley", result.Publisher);
            Assert.AreEqual(2019, result.Year);
        }

        /// <summary>
        /// Test 3: GetEditionById returns null when edition not found.
        /// </summary>
        [TestMethod]
        public void GetEditionById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            this.mockEditionRepository.Stub(x => x.GetById(999)).Return(null);

            // Act
            var result = this.editionService.GetEditionById(999);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Test 4: CreateEdition throws exception when edition is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateEdition_WithNullEdition_ThrowsArgumentNullException()
        {
            // Act
            this.editionService.CreateEdition(null);
        }

        /// <summary>
        /// Test 5: CreateEdition throws exception when book not found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateEdition_WithInvalidBookId_ThrowsException()
        {
            // Arrange
            var edition = new Edition
            {
                BookId = 999,
                Publisher = "Test Publisher",
                Year = 2023,
                EditionNumber = 1,
                PageCount = 300,
                BookType = "Hardcover"
            };

            this.mockBookRepository.Stub(x => x.GetById(999)).Return(null);

            // Act
            this.editionService.CreateEdition(edition);
        }

        /// <summary>
        /// Test 6: CreateEdition with valid data calls repository.
        /// </summary>
        [TestMethod]
        public void CreateEdition_WithValidData_CallsRepositoryAdd()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Test Book" };
            var edition = new Edition
            {
                BookId = 1,
                Publisher = "Test Publisher",
                Year = 2023,
                EditionNumber = 1,
                PageCount = 300,
                BookType = "Hardcover"
            };

            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);

            // Act
            this.editionService.CreateEdition(edition);

            // Assert
            this.mockEditionRepository.AssertWasCalled(x => x.Add(Arg<Edition>.Is.Anything));
        }

        /// <summary>
        /// Test 7: UpdateEdition calls repository update method.
        /// </summary>
        [TestMethod]
        public void UpdateEdition_WithValidData_CallsRepositoryUpdate()
        {
            // Arrange
            var edition = new Edition
            {
                Id = 1,
                BookId = 1,
                Publisher = "Updated Publisher",
                Year = 2023,
                PageCount=90
            };

            // Act
            this.editionService.UpdateEdition(edition);

            // Assert
            this.mockEditionRepository.AssertWasCalled(x => x.Update(edition));
        }

        /// <summary>
        /// Test 8: DeleteEdition calls repository delete method.
        /// </summary>
        [TestMethod]
        public void DeleteEdition_WithValidId_CallsRepositoryDelete()
        {
            // Arrange
            this.mockEditionRepository.Stub(x => x.GetById(1))
                .Return(new Edition { Id = 1, BookId = 1, Publisher = "Test" });

            // Act
            this.editionService.DeleteEdition(1);

            // Assert
            this.mockEditionRepository.AssertWasCalled(x => x.Delete(1));
        }

        /// <summary>
        /// Test 9: GetEditionsByBook returns editions of a specific book.
        /// </summary>
        [TestMethod]
        public void GetEditionsByBook_WithValidBookId_ReturnsBookEditions()
        {
            // Arrange
            var editions = new List<Edition>
            {
                new Edition { Id = 1, BookId = 1, Publisher = "Publisher A", Year = 2020 },
                new Edition { Id = 2, BookId = 1, Publisher = "Publisher B", Year = 2022 }
            };

            this.mockEditionRepository.Stub(x => x.GetByBookId(1)).Return(editions);

            // Act
            var result = this.editionService.GetEditionsByBook(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(e => e.BookId == 1));
        }

        /// <summary>
        /// Test 10: GetEditionsByPublisher returns editions by specific publisher.
        /// </summary>
        [TestMethod]
        public void GetEditionsByPublisher_WithValidPublisher_ReturnsMatchingEditions()
        {
            // Arrange
            var editions = new List<Edition>
            {
                new Edition { Id = 1, BookId = 1, Publisher = "O'Reilly", Year = 2020 },
                new Edition { Id = 2, BookId = 2, Publisher = "O'Reilly", Year = 2021 }
            };

            this.mockEditionRepository.Stub(x => x.GetByPublisher("O'Reilly")).Return(editions);

            // Act
            var result = this.editionService.GetEditionsByPublisher("O'Reilly");

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(e => e.Publisher == "O'Reilly"));
        }

        /// <summary>
        /// Test 11: Database integration - insert edition into database.
        /// </summary>
        [TestMethod]
        public void InsertEdition_IntoDatabase_SuccessfullyStored()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realEditionRepository = new EditionDataService(context);
                var realBookRepository = new BookDataService(context);
                var realService = new EditionService(realEditionRepository, realBookRepository);

                // Create a book first
                var author = new Author { FirstName = "Test", LastName = "Author" };
                var domain = new BookDomain { Id = 1, Name = "Science" };
                var book = new Book
                {
                    Title = "Test Book",
                    ISBN = "1111111111",
                    TotalCopies = 5,
                    Authors = new List<Author> { author },
                    Domains = new List<BookDomain> { domain }
                };

                var existingBook = context.Books.FirstOrDefault(b => b.ISBN == "1111111111");
                if (existingBook == null)
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                }

                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == "1111111111");

                var edition = new Edition
                {
                    BookId = createdBook.Id,
                    Publisher = "Test Publisher",
                    Year = 2023,
                    EditionNumber = 1,
                    PageCount = 350,
                    BookType = "Paperback"
                };

                // Act
                realService.CreateEdition(edition);

                // Assert
                var retrievedEdition = context.Editions
                    .FirstOrDefault(e => e.Publisher == "Test Publisher");

                Assert.IsNotNull(retrievedEdition, "Edition should exist in database");
                Assert.AreEqual(createdBook.Id, retrievedEdition.BookId);
                Assert.IsTrue(retrievedEdition.Id > 0, "Edition should have an ID");

                // Cleanup
                context.Editions.Remove(retrievedEdition);
                context.SaveChanges();

                context.Books.Remove(createdBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 12: Database integration - update edition in database.
        /// </summary>
        [TestMethod]
        public void UpdateEdition_InDatabase_ChangesPersisted()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realEditionRepository = new EditionDataService(context);
                var realBookRepository = new BookDataService(context);
                var realService = new EditionService(realEditionRepository, realBookRepository);

                // Create a book
                var author = new Author { FirstName = "Test", LastName = "Author" };
                var domain = new BookDomain { Id = 1, Name = "Science" };
                var book = new Book
                {
                    Title = "Test Book 2",
                    ISBN = "2222222222",
                    TotalCopies = 5,
                    Authors = new List<Author> { author },
                    Domains = new List<BookDomain> { domain }
                };

                var existingBook = context.Books.FirstOrDefault(b => b.ISBN == "2222222222");
                if (existingBook == null)
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                }

                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == "2222222222");

                var edition = new Edition
                {
                    BookId = createdBook.Id,
                    Publisher = "Original Publisher",
                    Year = 2020,
                    EditionNumber = 1,
                    PageCount = 300,
                    BookType = "Hardcover"
                };

                realService.CreateEdition(edition);

                var insertedEdition = context.Editions
                    .FirstOrDefault(e => e.Publisher == "Original Publisher");
                Assert.IsNotNull(insertedEdition);

                // Modify edition
                insertedEdition.Publisher = "Updated Publisher";
                insertedEdition.Year = 2023;

                // Act
                realService.UpdateEdition(insertedEdition);

                // Assert
                var updatedEdition = context.Editions.Find(insertedEdition.Id);
                Assert.AreEqual("Updated Publisher", updatedEdition.Publisher);
                Assert.AreEqual(2023, updatedEdition.Year);

                // Cleanup
                context.Editions.Remove(updatedEdition);
                context.SaveChanges();

                context.Books.Remove(createdBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 13: Database integration - delete edition from database.
        /// </summary>
        [TestMethod]
        public void DeleteEdition_FromDatabase_IsRemoved()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realEditionRepository = new EditionDataService(context);
                var realBookRepository = new BookDataService(context);
                var realService = new EditionService(realEditionRepository, realBookRepository);

                // Create a book
                var author = new Author { FirstName = "Test", LastName = "Author" };
                var domain = new BookDomain { Id = 1, Name = "Science" };
                var book = new Book
                {
                    Title = "Test Book 3",
                    ISBN = "3333333333",
                    TotalCopies = 5,
                    Authors = new List<Author> { author },
                    Domains = new List<BookDomain> { domain }
                };

                var existingBook = context.Books.FirstOrDefault(b => b.ISBN == "3333333333");
                if (existingBook == null)
                {
                    context.Books.Add(book);
                    context.SaveChanges();
                }

                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == "3333333333");

                var edition = new Edition
                {
                    BookId = createdBook.Id,
                    Publisher = "Delete Test Publisher",
                    Year = 2020,
                    EditionNumber = 1,
                    PageCount = 300,
                    BookType = "Hardcover"
                };

                realService.CreateEdition(edition);

                var insertedEdition = context.Editions
                    .FirstOrDefault(e => e.Publisher == "Delete Test Publisher");
                Assert.IsNotNull(insertedEdition);
                int editionId = insertedEdition.Id;

                // Act
                realService.DeleteEdition(editionId);

                // Assert
                var deletedEdition = context.Editions.Find(editionId);
                Assert.IsNull(deletedEdition, "Edition should be deleted from database");

                // Cleanup
                context.Books.Remove(createdBook);
                context.SaveChanges();
            }
        }
    }
}