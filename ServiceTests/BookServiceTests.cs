// <copyright file="BookServiceTests.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace ServiceTests
{
    using Data;
    using Data.Repositories;
    using Domain.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Lifetime;
    using System.Security.Policy;
    using System.Threading.Tasks;

    /// <summary>
    /// Unit tests for BookService with Rhino Mocks.
    /// </summary>
    [TestClass]
    public class BookServiceTests
    {
        private IBook mockBookRepository;
        private IBookDomain mockBookDomainRepository;
        private LibraryConfiguration mockConfigRepository;
        private BookService bookService;

        /// <summary>
        /// Initializes test fixtures before each test with mocked dependencies.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockBookRepository = MockRepository.GenerateStub<IBook>();
            this.mockBookDomainRepository = MockRepository.GenerateStub<IBookDomain>();
            this.mockConfigRepository = new LibraryConfiguration();

            this.bookService = new BookService(
                this.mockBookRepository,
                this.mockBookDomainRepository,
                this.mockConfigRepository);
        }

        /// <summary>
        /// Test 1: GetAllBooks returns list of books from repository.
        /// </summary>
        [TestMethod]
        public void GetAllBooks_WhenCalled_ReturnsAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book One" },
                new Book { Id = 2, Title = "Book Two" },
                new Book { Id = 3, Title = "Book Three" }
            };

            this.mockBookRepository.Stub(x => x.GetAll()).Return(books);

            // Act
            var result = this.bookService.GetAllBooks();

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(b => b.Title == "Book One"));
        }

        /// <summary>
        /// Test 2: GetBookById returns correct book when found.
        /// </summary>
        [TestMethod]
        public void GetBookById_WithValidId_ReturnsCorrectBook()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Test Book", ISBN = "123-456" };
            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);

            // Act
            var result = this.bookService.GetBookById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Test Book", result.Title);
        }

        /// <summary>
        /// Test 3: GetBooksByDomain returns books in domain and subdomains.
        /// </summary>
        [TestMethod]
        public void GetBooksByDomain_WithValidDomainId_ReturnsBooks()
        {
            // Arrange
            var parentDomain = new BookDomain { Id = 1, Name = "Science" };
            var childDomain = new BookDomain { Id = 2, Name = "Physics", ParentDomainId = 1 };

            var booksInParent = new List<Book>
            {
                new Book { Id = 1, Title = "General Science" }
            };

            var booksInChild = new List<Book>
            {
                new Book { Id = 2, Title = "Physics Basics" }
            };

            this.mockBookDomainRepository.Stub(x => x.GetById(1)).Return(parentDomain);
            this.mockBookDomainRepository.Stub(x => x.GetSubdomains(1))
                .Return(new List<BookDomain> { childDomain });
            this.mockBookDomainRepository.Stub(x => x.GetSubdomains(2))
                .Return(new List<BookDomain>());

            this.mockBookRepository.Stub(x => x.GetBooksByDomain(1)).Return(booksInParent);
            this.mockBookRepository.Stub(x => x.GetBooksByDomain(2)). Return(booksInChild);

            // Act
            var result = this.bookService.GetBooksByDomain(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(b => b.Title == "General Science"));
            Assert.IsTrue(result.Any(b => b.Title == "Physics Basics"));
        }

        /// <summary>
        /// Test 4: CreateBook throws exception when title is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateBook_WithNullTitle_ThrowsArgumentException()
        {
            // Arrange
            var book = new Book { Id = 1, Title = null };

            // Act
            this.bookService.CreateBook(book);
        }

        /// <summary>
        /// Test 5: CreateBook throws exception when exceeds max domains.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateBook_WithExceededDomains_ThrowsInvalidOperationException()
        {
            // Arrange
            var domains = new List<BookDomain>
            {
                new BookDomain { Id = 1, Name = "Domain1" },
                new BookDomain { Id = 2, Name = "Domain2" },
                new BookDomain { Id = 3, Name = "Domain3" },
                new BookDomain { Id = 4, Name = "Domain4" }
            };

            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                Domains = domains
            };

            
            this.bookService.CreateBook(book);
        }

        /// <summary>
        /// Test 6: ValidateBookDomains returns false when exceeding max domains.
        /// </summary>
        [TestMethod]
        public void ValidateBookDomains_WithExceededDomains_ReturnsFalse()
        {
            // Arrange
            var domains = new List<BookDomain>
            {
                new BookDomain { Id = 1, Name = "Domain1" },
                new BookDomain { Id = 2, Name = "Domain2" },
                new BookDomain { Id = 3, Name = "Domain3" },
                new BookDomain { Id = 4, Name = "Domain4" }
            };

            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                Domains = domains
            };

            // Act
            var result = this.bookService.ValidateBookDomains(book);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test 7: GetAvailableCopies returns correct count.
        /// </summary>
        [TestMethod]
        public void GetAvailableCopies_WithValidBook_ReturnsCorrectCount()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                TotalCopies = 10,
                ReadingRoomOnlyCopies = 2,
                BorrowingRecords = new List<Borrowing>
                {
                    new Borrowing { Id = 1, ReturnDate = null },
                    new Borrowing { Id = 2, ReturnDate = null }
                }
            };

            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);

            // Act
            var result = this.bookService.GetAvailableCopies(1);

            // Assert
            Assert.AreEqual(6, result); // 10 - 2 - 2 = 6
        }

        /// <summary>
        /// Test 8: GetAvailableBooks returns only loanable books.
        /// </summary>
        [TestMethod]
        public void GetAvailableBooks_WhenCalled_ReturnsAvailableBooks()
        {
            // Arrange
            var availableBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Available Book 1" },
                new Book { Id = 2, Title = "Available Book 2" }
            };

            this.mockBookRepository.Stub(x => x.GetAvailableBooks()).Return(availableBooks);

            // Act
            var result = this.bookService.GetAvailableBooks();

            // Assert
            Assert.AreEqual(2, result.Count());
        }
        
        /// <summary>
        /// Test 9: CreateBook with zero copies throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateBook_ZeroCopies_ThrowsArgumentException()
        {
            // Arrange
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 0,
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } },
                Domains = new List<BookDomain> { new BookDomain { Id = 1, Name = "Science" } }
            };

            // Act
            this.bookService.CreateBook(book);

            // Assert - Exception should be thrown above
        }

        /// <summary>
        /// Test 10: CreateBook with null domains throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateBook_WithNullDomains_ThrowsArgumentException()
        {
            // Arrange
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } },
                Domains = null
            };

            // Act
            this.bookService.CreateBook(book);

            // Assert - Exception should be thrown above
        }

        /// <summary>
        /// Test 11: CreateBook with empty domains throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateBook_WithEmptyDomains_ThrowsArgumentException()
        {
            // Arrange
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } },
                Domains = new List<BookDomain>()
            };

            // Act
            this.bookService.CreateBook(book);

            // Assert - Exception should be thrown above
        }

        /// <summary>
        /// Test 12: CreateBook with valid data succeeds.
        /// </summary>
        [TestMethod]
        public void CreateBook_WithValidData_SuccessfullyCreates()
        {
            // Arrange
            var domain = new BookDomain { Id = 1, Name = "Science" };
            var book = new Book
            {
                Title = "Valid Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } },
                Domains = new List<BookDomain> { domain }
            };

            this.mockBookRepository.Stub(x => x.GetByISBN("1234567890")).Return(null);

            // Act
            this.bookService.CreateBook(book);

            // Assert - No exception means success
            this.mockBookRepository.AssertWasCalled(x => x.Add(Arg<Book>.Is.Anything));
        }

        /// <summary>
        /// Test 13: CreateBook with negative reading room copies throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateBook_WithNegativeReadingRoomCopies_ThrowsArgumentException()
        {
            // Arrange
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = -1,
                Domains = new List<BookDomain> { new BookDomain { Id = 1, Name = "Science" } }
            };

            // Act
            this.bookService.CreateBook(book);

            // Assert - Exception should be thrown above
        }

        /// <summary>
        /// Test 14: CreateBook with reading room copies exceeding total throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateBook_WithReadingRoomCopiesExceedingTotal_ThrowsArgumentException()
        {
            // Arrange
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = 10,
                Domains = new List<BookDomain> { new BookDomain { Id = 1, Name = "Science" } }
            };

            // Act
            this.bookService.CreateBook(book);

            // Assert - Exception should be thrown above
        }

        #region Database Integration Tests

        /// <summary>
        /// Test 15: CreateBook inserts book into database successfully.
        /// Integration test with real database.
        /// </summary>
        [TestMethod]
        public void CreateBook_IntoDatabase_SuccessfullyStored()
        {
            // Arrange - Use real database context and repository
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDataService(context);
                var realDomainRepository = new BookDomainDataService(context);
                var realService = new BookService(realRepository, realDomainRepository, this.mockConfigRepository);

                var domain = new BookDomain { Id = 1, Name = "Science" };
                var author = new Author { FirstName = "John", LastName = "Smith" };
                var book = new Book
                {
                    Title = "Integration Test Book",
                    ISBN = "0001001111",
                    Description = "Test book for database integration",
                    TotalCopies = 5,
                    ReadingRoomOnlyCopies = 1,
                    Domains = new List<BookDomain> { domain },
                    Authors = new List<Author> { author }
                };

                // Act
                realService.CreateBook(book);

                // Assert - Verify the book was inserted into database
                var retrievedBook = context.Books
                    .FirstOrDefault(b => b.ISBN == "0001001111");

                Assert.IsNotNull(retrievedBook, "Book should exist in database");
                Assert.AreEqual("Integration Test Book", retrievedBook.Title);
                Assert.AreEqual("0001001111", retrievedBook.ISBN);
                Assert.AreEqual(5, retrievedBook.TotalCopies);
                Assert.AreEqual(1, retrievedBook.ReadingRoomOnlyCopies);
                Assert.IsTrue(retrievedBook.Id > 0, "Book should have been assigned an ID");

                // Cleanup
                context.Books.Remove(retrievedBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 16: CreateBook with multiple domains stores all domains.
        /// </summary>
        [TestMethod]
        public void CreateBook_WithMultipleDomains_StoresAllDomains()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDataService(context);
                var realDomainRepository = new BookDomainDataService(context);
                var realService = new BookService(realRepository, realDomainRepository, this.mockConfigRepository);
                var author = new Author { FirstName = "John", LastName = "Smith" };
                var domain1 = new BookDomain { Id = 1, Name = "Science" };
                var domain2 = new BookDomain { Id = 2, Name = "Technology" };

                var book = new Book
                {
                    Title = "Multi-Domain Test Book",
                    ISBN = "123464",
                    TotalCopies = 3,
                    Domains = new List<BookDomain> { domain1, domain2 },
                    Authors = new List<Author> { author }
                };

                // Act
                realService.CreateBook(book);

                // Assert
                var retrievedBook = context.Books
                    .FirstOrDefault(b => b.ISBN == "123464");

                Assert.IsNotNull(retrievedBook);
                Assert.AreEqual(2, retrievedBook.Domains.Count, "Book should have 2 domains");
                Assert.IsTrue(retrievedBook.Domains.Any(d => d.Name == "Science"));
                Assert.IsTrue(retrievedBook.Domains.Any(d => d.Name == "Technology"));

                // Cleanup
                context.Books.Remove(retrievedBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 17: CreateBook with authors stores author relationships.
        /// </summary>
        [TestMethod]
        public void CreateBook_WithAuthors_StoresAuthorRelationships()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDataService(context);
                var realDomainRepository = new BookDomainDataService(context);
                var realService = new BookService(realRepository, realDomainRepository, this.mockConfigRepository);

                var author1 = new Author { FirstName = "John", LastName = "Smith" };
                var author2 = new Author { FirstName = "Jane", LastName = "Doe" };
                var domain = new BookDomain { Id = 1, Name = "Science" };

                var book = new Book
                {
                    Title = "Multi-Author Test Book",
                    ISBN = "00011001",
                    TotalCopies = 4,
                    Authors = new List<Author> { author1, author2 },
                    Domains = new List<BookDomain> { domain }
                };

                // Act
                realService.CreateBook(book);

                // Assert
                var retrievedBook = context.Books
                    .FirstOrDefault(b => b.ISBN == "00011001");

                Assert.IsNotNull(retrievedBook);
                Assert.AreEqual(2, retrievedBook.Authors.Count, "Book should have 2 authors");
                Assert.IsTrue(retrievedBook.Authors.Any(a => a.FirstName == "John"));
                Assert.IsTrue(retrievedBook.Authors.Any(a => a.FirstName == "Jane"));

                // Cleanup
                context.Books.Remove(retrievedBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 18: GetBookById from database returns correct book.
        /// </summary>
        [TestMethod]
        public void GetBookById_FromDatabase_ReturnsCorrectBook()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDataService(context);
                var realDomainRepository = new BookDomainDataService(context);
                var realService = new BookService(realRepository, realDomainRepository, this.mockConfigRepository);
                var author = new Author { FirstName = "John", LastName = "Smith" };
                var domain = new BookDomain { Id = 1, Name = "Science" };
                var book = new Book
                {
                    Title = "GetById Test Book",
                    ISBN = "123001",
                    TotalCopies = 2,
                    Domains = new List<BookDomain> { domain },
                    Authors = new List<Author>{ author }
                };

                realService.CreateBook(book);

                var insertedBook = context.Books
                    .FirstOrDefault(b => b.ISBN == "123001");
                Assert.IsNotNull(insertedBook);
                int bookId = insertedBook.Id;

                // Act
                var retrievedBook = realService.GetBookById(bookId);

                // Assert
                Assert.IsNotNull(retrievedBook);
                Assert.AreEqual(bookId, retrievedBook.Id);
                Assert.AreEqual("GetById Test Book", retrievedBook.Title);
                Assert.AreEqual("123001", retrievedBook.ISBN);

                // Cleanup
                context.Books.Remove(insertedBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 19: UpdateBook persists changes to database.
        /// </summary>
        [TestMethod]
        public void UpdateBook_InDatabase_PersistsChanges()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDataService(context);
                var realDomainRepository = new BookDomainDataService(context);
                var realService = new BookService(realRepository, realDomainRepository, this.mockConfigRepository);
                var author = new Author { FirstName = "John", LastName = "Smith" };
                var domain = new BookDomain { Id = 1, Name = "Science" };
                var book = new Book
                {
                    Title = "Original Title",
                    ISBN = "0987001",
                    Description = "Original description",
                    TotalCopies = 5,
                    Domains = new List<BookDomain> { domain },
                    Authors = new List<Author>{ author }
                };

                realService.CreateBook(book);

                var insertedBook = context.Books
                    .FirstOrDefault(b => b.ISBN == "0987001");
                Assert.IsNotNull(insertedBook);

                // Modify the book
                insertedBook.Title = "Updated Title";
                insertedBook.Description = "Updated description";
                insertedBook.TotalCopies = 10;

                // Act
                realService.UpdateBook(insertedBook);

                // Assert - Verify changes were persisted
                var updatedBook = context.Books.Find(insertedBook.Id);
                Assert.AreEqual("Updated Title", updatedBook.Title);
                Assert.AreEqual("Updated description", updatedBook.Description);
                Assert.AreEqual(10, updatedBook.TotalCopies);

                // Cleanup
                context.Books.Remove(updatedBook);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 20: DeleteBook removes book from database.
        /// </summary>
        [TestMethod]
        public void DeleteBook_FromDatabase_IsRemoved()
        {
            // Arrange
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDataService(context);
                var realDomainRepository = new BookDomainDataService(context);
                var realService = new BookService(realRepository, realDomainRepository, this.mockConfigRepository);
                var author = new Author { FirstName = "John", LastName = "Smith" };
                var domain = new BookDomain { Id = 1, Name = "Science" };
                var book = new Book
                {
                    Title = "Delete Test Book",
                    ISBN = "9876001",
                    TotalCopies = 3,
                    Domains = new List<BookDomain> { domain },
                    Authors = new List<Author> { author}
                };

                realService.CreateBook(book);

                var insertedBook = context.Books
                    .FirstOrDefault(b => b.ISBN == "9876001");
                Assert.IsNotNull(insertedBook);
                int bookId = insertedBook.Id;

                // Act
                realService.DeleteBook(bookId);

                // Assert
                var deletedBook = context.Books.Find(bookId);
                Assert.IsNull(deletedBook, "Book should be deleted from database");
            }
        }

        /// <summary>
        /// Test: LoanExtension with valid request succeeds.
        /// </summary>
       

        #endregion
    }
}