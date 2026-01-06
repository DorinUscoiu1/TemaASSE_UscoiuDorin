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
            this.mockBookRepository.Stub(x => x.GetBooksByDomain(2)).Return(booksInChild);

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
            var domain = new BookDomain { Id = 1, Name = "Science" };
            var book = new Book { Id = 1, Title = null, Domains = new List<BookDomain> { domain } };

            // Act
            this.bookService.CreateBook(book, new List<int> { 1 });
        }

        /// <summary>
        /// Test 5: CreateBook throws exception when exceeds max domains.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateBook_WithExceededDomains_ThrowsInvalidOperationException()
        {
            // Arrange
            var domainIds = new List<int> { 1, 2, 3, 4 };

            // Act
            this.bookService.CreateBook(
                new Book { Title = "Test Book" }, 
                domainIds);
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
            Assert.AreEqual(6, result);
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
            var domain = new BookDomain { Id = 1, Name = "Science" };
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 0,
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } },
                Domains = new List<BookDomain> { domain }
            };

            // Act
            this.bookService.CreateBook(book, new List<int> { 1 });
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
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } }
            };

            // Act
            this.bookService.CreateBook(book, null);
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
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } }
            };

            // Act
            this.bookService.CreateBook(book, new List<int>());
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
            this.mockBookDomainRepository.Stub(x => x.GetById(1)).Return(domain);

            // Act
            this.bookService.CreateBook(book, new List<int> { 1 });

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
            var domain = new BookDomain { Id = 1, Name = "Science" };
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = -1,
                Domains = new List<BookDomain> { domain }
            };

            // Act
            this.bookService.CreateBook(book, new List<int> { 1 });
        }

        /// <summary>
        /// Test 14: CreateBook with reading room copies exceeding total throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public void CreateBook_WithReadingRoomCopiesExceedingTotal_ThrowsArgumentException()
        {
            // Arrange
            var domain = new BookDomain { Id = 1, Name = "Science" };
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = 10,
                Domains = new List<BookDomain> { domain }
            };

            // Act
            this.bookService.CreateBook(book, new List<int> { 1 });
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

                var domain = new BookDomain { Name = "Science" };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == "Science");
                Assert.IsNotNull(createdDomain);

                var author = new Author { FirstName = "John", LastName = "Smith" };
                var book = new Book
                {
                    Title = "Integration Test Book",
                    ISBN = "0001001111",
                    Description = "Test book for database integration",
                    TotalCopies = 5,
                    ReadingRoomOnlyCopies = 1,
                    Authors = new List<Author> { author }
                };

                // Act
                realService.CreateBook(book, new List<int> { createdDomain.Id });

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
                context.Domains.Remove(createdDomain);
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

                var domain1 = new BookDomain { Name = "Science" };
                var domain2 = new BookDomain { Name = "Technology" };
                context.Domains.Add(domain1);
                context.Domains.Add(domain2);
                context.SaveChanges();

                var createdDomain1 = context.Domains.FirstOrDefault(d => d.Name == "Science");
                var createdDomain2 = context.Domains.FirstOrDefault(d => d.Name == "Technology");

                var author = new Author { FirstName = "John", LastName = "Smith" };
                var book = new Book
                {
                    Title = "Multi-Domain Test Book",
                    ISBN = "123464",
                    TotalCopies = 3,
                    Authors = new List<Author> { author }
                };

                // Act
                realService.CreateBook(book, new List<int> { createdDomain1.Id, createdDomain2.Id });

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
                context.Domains.Remove(createdDomain1);
                context.Domains.Remove(createdDomain2);
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

                var domain = new BookDomain { Name = "Science" };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == "Science");

                var author1 = new Author { FirstName = "John", LastName = "Smith" };
                var author2 = new Author { FirstName = "Jane", LastName = "Doe" };

                var book = new Book
                {
                    Title = "Multi-Author Test Book",
                    ISBN = "00011001",
                    TotalCopies = 4,
                    Authors = new List<Author> { author1, author2 }
                };

                // Act
                realService.CreateBook(book, new List<int> { createdDomain.Id });

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
                context.Domains.Remove(createdDomain);
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

                var domain = new BookDomain { Name = "Science" };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == "Science");

                var author = new Author { FirstName = "John", LastName = "Smith" };
                var book = new Book
                {
                    Title = "GetById Test Book",
                    ISBN = "123001",
                    TotalCopies = 2,
                    Authors = new List<Author> { author }
                };

                realService.CreateBook(book, new List<int> { createdDomain.Id });

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
                context.Domains.Remove(createdDomain);
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

                var domain = new BookDomain { Name = "Science" };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == "Science");

                var author = new Author { FirstName = "John", LastName = "Smith" };
                var book = new Book
                {
                    Title = "Original Title",
                    ISBN = "0987001",
                    Description = "Original description",
                    TotalCopies = 5,
                    Authors = new List<Author> { author }
                };

                realService.CreateBook(book, new List<int> { createdDomain.Id });

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
                context.Domains.Remove(createdDomain);
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

                var domain = new BookDomain { Name = "Science" };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == "Science");

                var author = new Author { FirstName = "John", LastName = "Smith" };
                var book = new Book
                {
                    Title = "Delete Test Book",
                    ISBN = "9876001",
                    TotalCopies = 3,
                    Authors = new List<Author> { author }
                };

                realService.CreateBook(book, new List<int> { createdDomain.Id });

                var insertedBook = context.Books
                    .FirstOrDefault(b => b.ISBN == "9876001");
                Assert.IsNotNull(insertedBook);
                int bookId = insertedBook.Id;

                // Act
                realService.DeleteBook(bookId);

                // Assert
                var deletedBook = context.Books.Find(bookId);
                Assert.IsNull(deletedBook, "Book should be deleted from database");

                // Cleanup
                context.Domains.Remove(createdDomain);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test 21: GetBooksByAuthor returns all books by specific author.
        /// </summary>
        [TestMethod]
        public void GetBooksByAuthor_WithValidAuthorId_ReturnsAuthorBooks()
        {
            // Arrange
            var authorBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Book by Author 1" },
                new Book { Id = 2, Title = "Book by Author 2" }
            };

            this.mockBookRepository.Stub(x => x.GetBooksByAuthor(1))
                .Return(authorBooks);

            // Act
            var result = this.bookService.GetBooksByAuthor(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(b => b.Title == "Book by Author 1"));
        }

        /// <summary>
        /// Test 22: GetBooksOrderedByAvailability returns books sorted by available copies.
        /// </summary>
        [TestMethod]
        public void GetBooksOrderedByAvailability_WhenCalled_ReturnsSortedBooks()
        {
            // Arrange
            var book1 = new Book { Id = 1, Title = "Book 1", TotalCopies = 10, ReadingRoomOnlyCopies = 0, BorrowingRecords = new List<Borrowing>() };
            var book2 = new Book { Id = 2, Title = "Book 2", TotalCopies = 10, ReadingRoomOnlyCopies = 0, BorrowingRecords = new List<Borrowing>() };
            
            // book1: 10 available, book2: 5 available
            for (int i = 0; i < 5; i++)
                book2.BorrowingRecords.Add(new Borrowing { ReturnDate = null });

            var availableBooks = new List<Book> { book1, book2 };
            this.mockBookRepository.Stub(x => x.GetAvailableBooks()).Return(availableBooks);

            // Act
            var result = this.bookService.GetBooksOrderedByAvailability();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Book 1", result.First().Title); // Most available first
            Assert.AreEqual("Book 2", result.Last().Title);
        }

        /// <summary>
        /// Test 23: IsbnExists returns true when book with ISBN exists.
        /// </summary>
        [TestMethod]
        public void IsbnExists_WithExistingISBN_ReturnsTrue()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Test Book", ISBN = "1234567890" };
            this.mockBookRepository.Stub(x => x.GetByISBN("1234567890")).Return(book);

            // Act
            var result = this.bookService.IsbnExists("1234567890");

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test 24: IsbnExists returns false when ISBN doesn't exist.
        /// </summary>
        [TestMethod]
        public void IsbnExists_WithNonExistentISBN_ReturnsFalse()
        {
            // Arrange
            this.mockBookRepository.Stub(x => x.GetByISBN("9999999999")).Return(null);

            // Act
            var result = this.bookService.IsbnExists("9999999999");

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test 25: IsbnExists returns false when ISBN is null or empty.
        /// </summary>
        [TestMethod]
        public void IsbnExists_WithNullOrEmptyISBN_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(this.bookService.IsbnExists(null));
            Assert.IsFalse(this.bookService.IsbnExists(string.Empty));
            Assert.IsFalse(this.bookService.IsbnExists("   "));
        }

        /// <summary>
        /// Test 26: GetTotalBooksCount returns correct count.
        /// </summary>
        [TestMethod]
        public void GetTotalBooksCount_WhenCalled_ReturnsCorrectCount()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1" },
                new Book { Id = 2, Title = "Book 2" },
                new Book { Id = 3, Title = "Book 3" },
                new Book { Id = 4, Title = "Book 4" },
                new Book { Id = 5, Title = "Book 5" }
            };

            this.mockBookRepository.Stub(x => x.GetAll()).Return(books);

            // Act
            var result = this.bookService.GetTotalBooksCount();

            // Assert
            Assert.AreEqual(5, result);
        }

       

        /// <summary>
        /// Test 28: GetBooksWithNoCopiesAvailable returns only books with zero available copies.
        /// </summary>
        [TestMethod]
        public void GetBooksWithNoCopiesAvailable_WhenCalled_ReturnsUnavailableBooks()
        {
            // Arrange
            var book1 = new Book { Id = 1, Title = "Available", TotalCopies = 5, ReadingRoomOnlyCopies = 0, BorrowingRecords = new List<Borrowing>() };
            var book2 = new Book { Id = 2, Title = "Unavailable", TotalCopies = 5, ReadingRoomOnlyCopies = 5, BorrowingRecords = new List<Borrowing>() };
            
            var books = new List<Book> { book1, book2 };
            this.mockBookRepository.Stub(x => x.GetAll()).Return(books);

            // Act
            var result = this.bookService.GetBooksWithNoCopiesAvailable();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(b => b.Title == "Unavailable"));
        }

        /// <summary>
        /// Test 29: GetReadingRoomOnlyBooks returns books available only in reading room.
        /// </summary>
        [TestMethod]
        public void GetReadingRoomOnlyBooks_WhenCalled_ReturnsReadingRoomBooks()
        {
            // Arrange
            var book1 = new Book { Id = 1, Title = "Regular Book", TotalCopies = 5, ReadingRoomOnlyCopies = 2 };
            var book2 = new Book { Id = 2, Title = "Reading Room Only", TotalCopies = 3, ReadingRoomOnlyCopies = 3 };
            
            var books = new List<Book> { book1, book2 };
            this.mockBookRepository.Stub(x => x.GetAll()).Return(books);

            // Act
            var result = this.bookService.GetReadingRoomOnlyBooks();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(b => b.Title == "Reading Room Only"));
        }

        /// <summary>
        /// Test 30: CanBorrowBook returns false when all copies are reading room only.
        /// </summary>
        [TestMethod]
        public void CanBorrowBook_WithAllReadingRoomOnly_ReturnsFalse()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Reading Room Book",
                TotalCopies = 10,
                ReadingRoomOnlyCopies = 10,
                BorrowingRecords = new List<Borrowing>()
            };

            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);

            // Act
            var result = this.bookService.CanBorrowBook(1);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test 31: CreateBook with duplicate ISBN throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateBook_WithDuplicateISBN_ThrowsException()
        {
            // Arrange
            var domain = new BookDomain { Id = 1, Name = "Science" };
            var existingBook = new Book { Id = 1, ISBN = "1234567890" };
            var newBook = new Book
            {
                Title = "New Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } }
            };

            this.mockBookRepository.Stub(x => x.GetByISBN("1234567890")).Return(existingBook);
            this.mockBookDomainRepository.Stub(x => x.GetById(1)).Return(domain);

            // Act
            this.bookService.CreateBook(newBook, new List<int> { 1 });
        }

        /// <summary>
        /// Test 32: CreateBook with ancestor-descendant domains throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateBook_WithAncestorDescendantDomains_ThrowsException()
        {
            // Arrange
            var parentDomain = new BookDomain { Id = 1, Name = "Science" };
            var childDomain = new BookDomain { Id = 2, Name = "Physics", ParentDomainId = 1 };

            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890",
                TotalCopies = 5,
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } }
            };

            this.mockBookRepository.Stub(x => x.GetByISBN("1234567890")).Return(null);
            this.mockBookDomainRepository.Stub(x => x.GetById(1)).Return(parentDomain);
            this.mockBookDomainRepository.Stub(x => x.GetById(2)).Return(childDomain);

            // Act & Assert
            this.bookService.CreateBook(book, new List<int> { 1, 2 });
        }

        /// <summary>
        /// Test 33: GetBooksInDomain returns books including subdomains.
        /// </summary>
        [TestMethod]
        public void GetBooksInDomain_WithSubdomains_IncludesAllBooks()
        {
            // Arrange
            var parentDomain = new BookDomain { Id = 1, Name = "Science" };
            var childDomain = new BookDomain { Id = 2, Name = "Physics", ParentDomainId = 1 };

            var booksInParent = new List<Book> { new Book { Id = 1, Title = "Science Book" } };
            var booksInChild = new List<Book> { new Book { Id = 2, Title = "Physics Book" } };

            this.mockBookDomainRepository.Stub(x => x.GetById(1)).Return(parentDomain);
            this.mockBookDomainRepository.Stub(x => x.GetSubdomains(1))
                .Return(new List<BookDomain> { childDomain });
            this.mockBookDomainRepository.Stub(x => x.GetSubdomains(2))
                .Return(new List<BookDomain>());

            this.mockBookRepository.Stub(x => x.GetBooksByDomain(1)).Return(booksInParent);
            this.mockBookRepository.Stub(x => x.GetBooksByDomain(2)).Return(booksInChild);

            // Act
            var result = this.bookService.GetBooksInDomain(1);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// Test 34: GetBooksDirectlyInDomain returns only books directly in domain (no subdomains).
        /// </summary>
        [TestMethod]
        public void GetBooksDirectlyInDomain_WhenCalled_ReturnsDomainBooksOnly()
        {
            // Arrange
            var domain = new BookDomain { Id = 1, Name = "Science" };
            var books = new List<Book> { new Book { Id = 1, Title = "Science Book" } };

            this.mockBookDomainRepository.Stub(x => x.GetById(1)).Return(domain);
            this.mockBookRepository.Stub(x => x.GetBooksByDomain(1)).Return(books);

            // Act
            var result = this.bookService.GetBooksDirectlyInDomain(1);

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        /// <summary>
        /// Test 36: Database - GetBooksInDomain with hierarchy includes subdomain books.
        /// </summary>
        [TestMethod]
        public void GetBooksInDomain_Database_IncludesSubdomainBooks()
        {
            using (var context = new LibraryDbContext())
            {
                var realRepository = new BookDataService(context);
                var realDomainRepository = new BookDomainDataService(context);
                var realService = new BookService(realRepository, realDomainRepository, this.mockConfigRepository);

                // Create domain hierarchy
                var parentDomain = new BookDomain { Name = "Science_Parent_" + Guid.NewGuid().ToString().Substring(0, 5) };
                var childDomain = new BookDomain { Name = "Physics_Child_" + Guid.NewGuid().ToString().Substring(0, 5), ParentDomainId = 0 };
                context.Domains.Add(parentDomain);
                context.SaveChanges();

                var createdParent = context.Domains.FirstOrDefault(d => d.Name == parentDomain.Name);
                Assert.IsNotNull(createdParent);

                childDomain.ParentDomainId = createdParent.Id;
                context.Domains.Add(childDomain);
                context.SaveChanges();

                var createdChild = context.Domains.FirstOrDefault(d => d.Name == childDomain.Name);
                Assert.IsNotNull(createdChild);

                // Create books
                var book1 = new Book
                {
                    Title = "Parent Domain Book",
                    ISBN = "555555",
                    TotalCopies = 5,
                    Authors = new List<Author> { new Author { FirstName = "Author", LastName = "One" } }
                };
                realService.CreateBook(book1, new List<int> { createdParent.Id });

                var book2 = new Book
                {
                    Title = "Child Domain Book",
                    ISBN = "666666",
                    TotalCopies = 5,
                    Authors = new List<Author> { new Author { FirstName = "Author", LastName = "Two" } }
                };
                realService.CreateBook(book2, new List<int> { createdChild.Id });

                // Act
                var result = realService.GetBooksInDomain(createdParent.Id);

                // Assert
                Assert.AreEqual(2, result.Count(), "Should include books from both parent and child domains");

                // Cleanup
                var createdBooks = context.Books.Where(b => b.Title == "Parent Domain Book" || b.Title == "Child Domain Book").ToList();
                foreach (var b in createdBooks) context.Books.Remove(b);
                context.SaveChanges();

                context.Domains.Remove(createdChild);
                context.Domains.Remove(createdParent);
                context.SaveChanges();
            }
        }

        #endregion
    }
}