// <copyright file="BorrowingServiceTests.cs" company="Transilvania University of Brasov">
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
    /// Unit tests for BorrowingService with Rhino Mocks.
    /// </summary>
    [TestClass]
    public class BorrowingServiceTests
    {
        private IBorrowing mockBorrowingRepository;
        private IBook mockBookRepository;
        private IReader mockReaderRepository;
        private LibraryConfiguration mockConfigRepository;
        private BorrowingService borrowingService;

        /// <summary>
        /// Initializes test fixtures before each test with mocked dependencies.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockBorrowingRepository = MockRepository.GenerateStub<IBorrowing>();
            this.mockBookRepository = MockRepository.GenerateStub<IBook>();
            this.mockReaderRepository = MockRepository.GenerateStub<IReader>();
            this.mockConfigRepository = new LibraryConfiguration();

            this.borrowingService = new BorrowingService(
                this.mockBorrowingRepository,
                this.mockBookRepository,
                this.mockReaderRepository,
                this.mockConfigRepository);
        }

        /// <summary>
        /// Test 1: BorrowBook throws exception when reader not found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BorrowBook_WithInvalidReader_ThrowsException()
        {
            // Arrange
            this.mockReaderRepository.Stub(x => x.GetById(999)).Return(null);
            this.mockBookRepository.Stub(x => x.GetById(1))
                .Return(new Book { Id = 1, Title = "Test Book" });

            // Act
            this.borrowingService.BorrowBook(999, 1, 14);
        }

        /// <summary>
        /// Test 2: BorrowBook throws exception when book not found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BorrowBook_WithInvalidBook_ThrowsException()
        {
            // Arrange
            this.mockReaderRepository.Stub(x => x.GetById(1))
                .Return(new Reader { Id = 1, FirstName = "John", LastName = "Doe" });
            this.mockBookRepository.Stub(x => x.GetById(999)).Return(null);

            // Act
            this.borrowingService.BorrowBook(1, 999, 14);
        }

        /// <summary>
        /// Test 3: GetActiveBorrowings returns active borrowings for reader.
        /// </summary>
        [TestMethod]
        public void GetActiveBorrowings_WithValidReaderId_ReturnsActiveBorrowings()
        {
            // Arrange
            var activeBorrowings = new List<Borrowing>
            {
                new Borrowing { Id = 1, ReaderId = 1, IsActive = true },
                new Borrowing { Id = 2, ReaderId = 1, IsActive = true }
            };

            this.mockBorrowingRepository.Stub(x => x.GetActiveBorrowingsByReader(1))
                .Return(activeBorrowings);

            // Act
            var result = this.borrowingService.GetActiveBorrowings(1);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(b => b.IsActive));
        }

        /// <summary>
        /// Test 4: GetOverdueBorrowings returns overdue records.
        /// </summary>
        [TestMethod]
        public void GetOverdueBorrowings_WhenCalled_ReturnsOverdueRecords()
        {
            // Arrange
            var overdueBorrowings = new List<Borrowing>
            {
                new Borrowing
                {
                    Id = 1,
                    DueDate = DateTime.Now.AddDays(-5),
                    IsActive = true
                },
                new Borrowing
                {
                    Id = 2,
                    DueDate = DateTime.Now.AddDays(-3),
                    IsActive = true
                }
            };

            this.mockBorrowingRepository.Stub(x => x.GetOverdueBorrowings())
                .Return(overdueBorrowings);

            // Act
            var result = this.borrowingService.GetOverdueBorrowings();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// Test 5: ExtendBorrowing throws exception when exceeding max extension days.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtendBorrowing_WithExceededExtensionDays_ThrowsException()
        {
            // Arrange
            var borrowing = new Borrowing
            {
                Id = 1,
                DueDate = DateTime.Now.AddDays(5),
                TotalExtensionDays = 5
            };

            this.borrowingService.ExtendBorrowing(1, 5,DateTime.Now);
        }

        /// <summary>
        /// Test 6: CanBorrowBook returns false when no available copies.
        /// </summary>
        [TestMethod]
        public void CanBorrowBook_WithNoAvailableCopies_ReturnsFalse()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                IsStaff = false
            };

            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = 5,
                BorrowingRecords = new List<Borrowing>()
            };

            this.mockReaderRepository.Stub(x => x.GetById(1)).Return(reader);
            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);

            // Act
            var result = this.borrowingService.CanBorrowBook(1, 1);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test 7: GetActiveBorrowingCount returns correct count.
        /// </summary>
        [TestMethod]
        public void GetActiveBorrowingCount_WithValidReaderId_ReturnsCorrectCount()
        {
            // Arrange
            var activeBorrowings = new List<Borrowing>
            {
                new Borrowing { Id = 1, ReaderId = 1, IsActive = true },
                new Borrowing { Id = 2, ReaderId = 1, IsActive = true },
                new Borrowing { Id = 3, ReaderId = 1, IsActive = true }
            };

            this.mockBorrowingRepository.Stub(x => x.GetActiveBorrowingsByReader(1))
                .Return(activeBorrowings);

            // Act
            var result = this.borrowingService.GetActiveBorrowingCount(1);

            // Assert
            Assert.AreEqual(3, result);
        }

        /// <summary>
        /// Test 8: CanBorrowBook returns true when all conditions met.
        /// </summary>
        [TestMethod]
        public void CanBorrowBook_WithAllConditionsMet_ReturnsTrue()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                IsStaff = false
            };

            var book = new Book
            {
                Id = 1,
                Title = "Test Book",
                TotalCopies = 10,
                ReadingRoomOnlyCopies = 2,
                BorrowingRecords = new List<Borrowing>(),
                Domains = new List<BookDomain>()
            };

            this.mockReaderRepository.Stub(x => x.GetById(1)).Return(reader);
            this.mockBookRepository.Stub(x => x.GetById(1)).Return(book);
            this.mockBorrowingRepository.Stub(x => x.GetActiveBorrowingsByReader(1))
                .Return(new List<Borrowing>());
            this.mockBorrowingRepository.Stub(x => x.GetBorrowingsByDateRange(
                Arg<DateTime>.Is.Anything, Arg<DateTime>.Is.Anything))
                .Return(new List<Borrowing>());
            this.mockBorrowingRepository.Stub(x => x.GetBorrowingsByBook(1))
                .Return(new List<Borrowing>());

            // Act
            var result = this.borrowingService.CanBorrowBook(1, 1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReturnBorrowing_WithAlreadyReturnedBorrowing_ThrowsException()
        {
            // Arrange
            var borrowing = new Borrowing
            {
                Id = 1,
                IsActive = false,
                ReturnDate = DateTime.Now.AddDays(-5)
            };

            this.mockBorrowingRepository.Stub(x => x.GetById(1)).Return(borrowing);

            // Act
            this.borrowingService.ReturnBorrowing(1, DateTime.Now);
        }


        [TestMethod]
        public void CompleteWorkflow_CreateBookAndLoan_Succeeds()
        {
            using (var context = new LibraryDbContext())
            {
                // Setup Repositories & Services
                var bookRepo = new BookDataService(context);
                var domainRepo = new BookDomainDataService(context);
                var readerRepo = new ReaderDataService(context);
                var borrowingRepo = new BorrowingDataService(context);
                var config = new LibraryConfiguration();

                var readerService = new ReaderService(readerRepo);
                var bookService = new BookService(bookRepo, domainRepo, config);
                var borrowingService = new BorrowingService(borrowingRepo, bookRepo, readerRepo, config);

                // 1. Creare Domeniu
                var domain = new BookDomain { Name = "Science" };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == "Science");
                Assert.IsNotNull(createdDomain);

                // 2. Creare Carte (Complex - prin Service)
                var book = new Book
                {
                    Title = "Test Book",
                    ISBN = "1234567893",
                    TotalCopies = 10,
                    Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } }
                };
                var existingBook = context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);
                if (existingBook == null)
                    bookService.CreateBook(book, new List<int> { createdDomain.Id });

                // Retrieve the created book with its ID
                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == "1234567893");
                Assert.IsNotNull(createdBook);

                // 3. Creare Cititor
                var reader = new Reader
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Address = "123 Main St",
                    PhoneNumber = "555-1234",
                    Email = "jane@example.com"
                };
                readerService.CreateReader(reader);
                context.SaveChanges();

                var createdReader = context.Readers.FirstOrDefault(r => r.Email == "jane@example.com");
                Assert.IsNotNull(createdReader);

                // 4. Creare Împrumut (Complex - prin Service)
                borrowingService.CreateBorrowings(createdReader.Id, new List<int> { createdBook.Id }, DateTime.Now, 14);

                // Assert
                var activeBorrowings = borrowingService.GetActiveBorrowings(createdReader.Id);
                Assert.AreEqual(1, activeBorrowings.Count());

                // Cleanup
                var borrowings = context.Borrowings.Where(b => b.BookId == createdBook.Id).ToList();
                foreach (var borrowing in borrowings)
                {
                    context.Borrowings.Remove(borrowing);
                }
                context.SaveChanges();

                context.Readers.Remove(createdReader);
                context.SaveChanges();

                context.Books.Remove(createdBook);
                context.SaveChanges();

                context.Domains.Remove(createdDomain);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void LoanExtension_ValidRequest_Succeeds()
        {
            using (var context = new LibraryDbContext())
            {
                // Setup Repositories & Services
                var bookRepo = new BookDataService(context);
                var domainRepo = new BookDomainDataService(context);
                var readerRepo = new ReaderDataService(context);
                var borrowingRepo = new BorrowingDataService(context);
                var config = new LibraryConfiguration();

                var readerService = new ReaderService(readerRepo);
                var domainService = new BookDomainService(domainRepo);
                var bookService = new BookService(bookRepo, domainRepo, config);
                var borrowingService = new BorrowingService(borrowingRepo, bookRepo, readerRepo, config);

                // 1. Creare Domeniu
                var domain = new BookDomain { Name = "Science" };
                domainService.CreateDomain(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == "Science");
                Assert.IsNotNull(createdDomain);

                // 2. Creare Carte (prin Service)
                var book = new Book
                {
                    Title = "Test Book",
                    ISBN = "1234567890",
                    TotalCopies = 10,
                    Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe" } }
                };
                var existingBook = context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);
                if (existingBook == null)
                    bookService.CreateBook(book, new List<int> { createdDomain.Id });

                // Retrieve the created book with its ID
                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == "1234567890");
                Assert.IsNotNull(createdBook);

                // 3. Creare Cititor
                var reader = new Reader
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Address = "123 Main St",
                    Email = "jane@example.com"
                };
                readerService.CreateReader(reader);
                context.SaveChanges();

                var createdReader = context.Readers.FirstOrDefault(r => r.Email == "jane@example.com");
                Assert.IsNotNull(createdReader);

                // 4. Creare Împrumut (prin Service)
                DateTime borrowingDate = DateTime.Now;
                borrowingService.CreateBorrowings(createdReader.Id, new List<int> { createdBook.Id }, borrowingDate, 14);

                // Retrieve the created borrowing
                var borrowing = borrowingRepo.GetActiveBorrowingsByReader(createdReader.Id).FirstOrDefault();
                Assert.IsNotNull(borrowing, "Împrumutul ar trebui să existe.");

                // 5. Extensie Împrumut (prin Service)
                int extensionDays = 7;
                DateTime extensionDate = DateTime.Now;
                borrowingService.ExtendBorrowing(borrowing.Id, extensionDays, extensionDate);

                // 6. Assert - Verificăm dacă datele au fost actualizate în DB
                var updatedBorrowing = borrowingRepo.GetById(borrowing.Id);

                DateTime expectedDueDate = borrowingDate.AddDays(14).AddDays(extensionDays);
                Assert.AreEqual(expectedDueDate.Date, updatedBorrowing.DueDate.Date);
                Assert.AreEqual(extensionDays, updatedBorrowing.TotalExtensionDays);
                Assert.AreEqual(extensionDate.Date, updatedBorrowing.LastExtensionDate.Value.Date);

                // Cleanup
                context.Borrowings.Remove(updatedBorrowing);
                context.SaveChanges();

                context.Readers.Remove(createdReader);
                context.SaveChanges();

                context.Books.Remove(createdBook);
                context.SaveChanges();

                context.Domains.Remove(createdDomain);
                context.SaveChanges();
            }
        }
        [TestMethod]
        public void StaffReader_HasDoublePrivileges_Succeeds()
        {
            using (var context = new LibraryDbContext())
            {
                // Setup Repositories & Services
                var bookRepo = new BookDataService(context);
                var domainRepo = new BookDomainDataService(context);
                var readerRepo = new ReaderDataService(context);
                var borrowingRepo = new BorrowingDataService(context);
                var config = new LibraryConfiguration();

                var readerService = new ReaderService(readerRepo);
                var bookService = new BookService(bookRepo, domainRepo, config);
                var borrowingService = new BorrowingService(borrowingRepo, bookRepo, readerRepo, config);

                // 1. Pregătire Domenii
                var domain1 = new BookDomain { Name = "Science_" + Guid.NewGuid().ToString().Substring(0, 5) };
                var domain2 = new BookDomain { Name = "Arts_" + Guid.NewGuid().ToString().Substring(0, 5) };
                context.Domains.Add(domain1);
                context.Domains.Add(domain2);
                context.SaveChanges();

                var createdDomain1 = context.Domains.FirstOrDefault(d => d.Name == domain1.Name);
                var createdDomain2 = context.Domains.FirstOrDefault(d => d.Name == domain2.Name);
                Assert.IsNotNull(createdDomain1);
                Assert.IsNotNull(createdDomain2);

                // 2. Creare 8 Cărți prin Service (pentru a avea stoc și domenii valide)
                var createdBookIds = new List<int>();
                for (int i = 0; i < 8; i++)
                {
                    var book = new Book
                    {
                        Title = $"Staff Privilege Book {i}",
                        ISBN = "99023" + (i+1).ToString(),
                        TotalCopies = 10,
                        Authors = new List<Author> { new Author { FirstName = "Author", LastName = "Test" } }
                    };

                    // Repartizăm cărțile în 2 domenii diferite pentru a respecta regula diversității (minim 2 domenii la 3+ cărți)
                    var domainId = (i < 4) ? createdDomain1.Id : createdDomain2.Id;
                    bookService.CreateBook(book, new List<int> { domainId });
                    
                    // Retrieve the created book to get its ID
                    var createdBook = context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);
                    if (createdBook != null)
                    {
                        createdBookIds.Add(createdBook.Id);
                    }
                }

                Assert.AreEqual(8, createdBookIds.Count, "All 8 books should be created");

                // 3. Creare Cititor de tip Staff (IsStaff = true)
                var staffReader = new Reader
                {
                    FirstName = "Staff",
                    LastName = "Member",
                    Address = "University Library",
                    Email = "staff." + Guid.NewGuid() + "@unitbv.ro",
                    IsStaff = true // <--- Aceasta este cheia testului
                };
                readerService.CreateReader(staffReader);
                context.SaveChanges();

                var createdReader = context.Readers.FirstOrDefault(r => r.Email == staffReader.Email);
                Assert.IsNotNull(createdReader);

                // 4. Încercare împrumut 6 cărți simultan
                // Dacă limita standard (C) este 3, Staff-ul ar trebui să poată lua 6 (C*2)
                var bookIdsToBorrow = createdBookIds.Take(6).ToList();
                DateTime borrowingDate = DateTime.Now;

                // Act
                // Dacă serviciul funcționează corect, nu va arunca excepție pentru IsStaff = true
                borrowingService.CreateBorrowings(createdReader.Id, bookIdsToBorrow, borrowingDate, 14);

                // 5. Assert
                var activeBorrowings = borrowingService.GetActiveBorrowings(createdReader.Id).ToList();
                Assert.AreEqual(6, activeBorrowings.Count, "Staff-ul ar fi trebuit să poată împrumuta 6 cărți.");

                // Verificăm că toate au data setată corect (fără overflow)
                Assert.IsTrue(activeBorrowings.All(b => b.BorrowingDate.Year >= 2025));

                // Cleanup - DELETE IN CORRECT ORDER: borrowings → reader → books → domains
                var allBorrowingsForReader = context.Borrowings.Where(b => b.ReaderId == createdReader.Id).ToList();
                foreach (var borrowing in allBorrowingsForReader)
                {
                    context.Borrowings.Remove(borrowing);
                }
                context.SaveChanges();

                context.Readers.Remove(createdReader);
                context.SaveChanges();

                var allBooksToRemove = context.Books.Where(b => createdBookIds.Contains(b.Id)).ToList();
                foreach (var book in allBooksToRemove)
                {
                    context.Books.Remove(book);
                }
                context.SaveChanges();

                context.Domains.Remove(createdDomain1);
                context.Domains.Remove(createdDomain2);
                context.SaveChanges();
            }
        }
        [TestMethod]
        public void BookAvailability_AfterLoans_UpdatesCorrectly()
        {
            using (var context = new LibraryDbContext())
            {
                // Setup Repositories & Services
                var bookRepo = new BookDataService(context);
                var domainRepo = new BookDomainDataService(context);
                var readerRepo = new ReaderDataService(context);
                var borrowingRepo = new BorrowingDataService(context);
                var config = new LibraryConfiguration();

                var readerService = new ReaderService(readerRepo);
                var bookService = new BookService(bookRepo, domainRepo, config);
                var borrowingService = new BorrowingService(borrowingRepo, bookRepo, readerRepo, config);

                // 1. Creare Domeniu
                var domainName = "AvailTest_" + Guid.NewGuid().ToString().Substring(0, 5);
                var domain = new BookDomain { Name = domainName };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == domainName);
                Assert.IsNotNull(createdDomain);

                // 2. Creare Carte prin Service (10 exemplare, 0 reading room only)
                var isbn = "345434543234";
                var book = new Book
                {
                    Title = "Availability Test Book",
                    ISBN = isbn,
                    TotalCopies = 10,
                    ReadingRoomOnlyCopies = 0,
                    Authors = new List<Author> { new Author { FirstName = "Test", LastName = "Author" } }
                };
                bookService.CreateBook(book, new List<int> { createdDomain.Id });
                context.SaveChanges();

                // Retrieve the created book from DB to get its actual ID
                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == isbn);
                Assert.IsNotNull(createdBook, "Book should be created");
                int bookId = createdBook.Id;

                // Verificare inițială - ar trebui să avem 10 exemplare disponibile
                int initialAvailable = createdBook.GetAvailableCopies();
                Assert.AreEqual(10, initialAvailable, 
                    $"Inițial ar trebui să fie 10 exemplare disponibile, dar sunt {initialAvailable}");

                // 3. Creare Cititor
                var readerEmail = "jane.avail@" + Guid.NewGuid().ToString().Substring(0, 8) + ".com";
                var reader = new Reader
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = readerEmail,
                    Address = "123 Main St",
                    PhoneNumber = "555-1234"
                };
                readerService.CreateReader(reader);
                context.SaveChanges();

                var createdReader = context.Readers.FirstOrDefault(r => r.Email == readerEmail);
                Assert.IsNotNull(createdReader);

                // 4. Executare Împrumut prin Service
                borrowingService.CreateBorrowings(createdReader.Id, new List<int> { createdBook.Id }, DateTime.Now, 14);
                context.SaveChanges();

                // 5. Reîncărcare Carte din DB pentru a include noile împrumuturi
                var bookAfterLoan = context.Books.Find(bookId);
                Assert.IsNotNull(bookAfterLoan);

                // Get active borrowings for verification
                var activeBorrowings = context.Borrowings
                    .Where(b => b.BookId == bookId && b.IsActive)
                    .ToList();
                
                int activeBorrowCount = activeBorrowings.Count;
                Assert.AreEqual(1, activeBorrowCount, "Should have exactly 1 active borrowing");

                // Assert - Calculul disponibilității: TotalCopies - ActiveLoans - ReadingRoomOnly
                // = 10 - 1 - 0 = 9
                int expectedAvailable = 10 - activeBorrowCount - 0;
                int actualAvailable = bookAfterLoan.GetAvailableCopies();
                
                Assert.AreEqual(expectedAvailable, actualAvailable,
                    $"După împrumut, ar trebui să mai fie {expectedAvailable} exemplare disponibile, dar sunt {actualAvailable}. " +
                    $"Total: {bookAfterLoan.TotalCopies}, Active: {activeBorrowCount}, ReadingRoom: {bookAfterLoan.ReadingRoomOnlyCopies}");

                // ==================== CLEANUP - DELETE IN CORRECT ORDER ====================
                // 1. Remove borrowings first
                var allBorrowings = context.Borrowings.Where(b => b.BookId == bookId).ToList();
                foreach (var borrowing in allBorrowings)
                {
                    context.Borrowings.Remove(borrowing);
                }
                context.SaveChanges();

                // 2. Remove reader
                context.Readers.Remove(createdReader);
                context.SaveChanges();

                // 3. Remove book
                context.Books.Remove(bookAfterLoan);
                context.SaveChanges();

                // 4. Remove domain
                context.Domains.Remove(createdDomain);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test: Multiple readers borrowing same book - availability decreases correctly.
        /// </summary>
        [TestMethod]
        public void MultipleReaders_BorrowSameBook_AvailabilityDecreases()
        {
            using (var context = new LibraryDbContext())
            {
                var bookRepo = new BookDataService(context);
                var domainRepo = new BookDomainDataService(context);
                var readerRepo = new ReaderDataService(context);
                var borrowingRepo = new BorrowingDataService(context);
                var config = new LibraryConfiguration();

                var readerService = new ReaderService(readerRepo);
                var bookService = new BookService(bookRepo, domainRepo, config);
                var borrowingService = new BorrowingService(borrowingRepo, bookRepo, readerRepo, config);

                // Create domain with unique name
                var domainName = "MultiReader_" + Guid.NewGuid().ToString().Substring(0, 5);
                var domain = new BookDomain { Name = domainName };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == domainName);
                Assert.IsNotNull(createdDomain, "Domain should be created");

                // Create book with unique ISBN
                var isbn = "234323432134";
                var book = new Book
                {
                    Title = "Multi-Reader Book",
                    ISBN = isbn,
                    TotalCopies = 5,
                    ReadingRoomOnlyCopies = 0,
                    Authors = new List<Author> { new Author { FirstName = "Test", LastName = "Author" } }
                };
                bookService.CreateBook(book, new List<int> { createdDomain.Id });
                context.SaveChanges();

                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == isbn);
                Assert.IsNotNull(createdBook, "Book should be created");

                // Initial availability should be 5
                int initialAvailable = createdBook.GetAvailableCopies();
                Assert.AreEqual(5, initialAvailable, $"Initial availability should be 5, but is {initialAvailable}");

                // Create 3 readers with unique emails
                var readerEmails = new List<string>();
                for (int i = 0; i < 3; i++)
                {
                    var email = $"reader{i}_{Guid.NewGuid()}@test.com";
                    var reader = new Reader
                    {
                        FirstName = $"Reader{i}",
                        LastName = "Test",
                        Email = email,
                        Address = $"Address {i}",
                        PhoneNumber = $"555-000{i}"
                    };
                    readerService.CreateReader(reader);
                    readerEmails.Add(email);
                }
                context.SaveChanges();

                // Retrieve readers using only emails (primitive type)
                var createdReaders = new List<Reader>();
                foreach (var email in readerEmails)
                {
                    var reader = context.Readers.FirstOrDefault(r => r.Email == email);
                    if (reader != null)
                    {
                        createdReaders.Add(reader);
                    }
                }
                Assert.AreEqual(3, createdReaders.Count, "All 3 readers should be created");

                // Reader 1 borrows 1 copy
                borrowingService.CreateBorrowings(createdReaders[0].Id, new List<int> { createdBook.Id }, DateTime.Now, 14);
                context.SaveChanges();
                
                var book1 = context.Books.Find(createdBook.Id);
                int available1 = book1.GetAvailableCopies();
                Assert.AreEqual(4, available1, $"After 1st borrow, should have 4 copies, but have {available1}");

                // Reader 2 borrows 1 copy
                borrowingService.CreateBorrowings(createdReaders[1].Id, new List<int> { createdBook.Id }, DateTime.Now, 14);
                context.SaveChanges();
                
                var book2 = context.Books.Find(createdBook.Id);
                int available2 = book2.GetAvailableCopies();
                Assert.AreEqual(3, available2, $"After 2nd borrow, should have 3 copies, but have {available2}");

                // Reader 3 borrows 1 copy
                borrowingService.CreateBorrowings(createdReaders[2].Id, new List<int> { createdBook.Id }, DateTime.Now, 14);
                context.SaveChanges();
                
                var book3 = context.Books.Find(createdBook.Id);
                int available3 = book3.GetAvailableCopies();
                Assert.AreEqual(2, available3, $"After 3rd borrow, should have 2 copies, but have {available3}");

                // Cleanup - correct order: borrowings → readers → books → domains
                var allBorrowings = context.Borrowings.Where(b => b.BookId == createdBook.Id).ToList();
                foreach (var borrowing in allBorrowings)
                {
                    context.Borrowings.Remove(borrowing);
                }
                context.SaveChanges();

                foreach (var reader in createdReaders)
                {
                    context.Readers.Remove(reader);
                }
                context.SaveChanges();

                context.Books.Remove(createdBook);
                context.SaveChanges();

                context.Domains.Remove(createdDomain);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Test: Return book increases availability correctly.
        /// </summary>
        [TestMethod]
        public void ReturnBook_IncreasesAvailability()
        {
            using (var context = new LibraryDbContext())
            {
                var bookRepo = new BookDataService(context);
                var domainRepo = new BookDomainDataService(context);
                var readerRepo = new ReaderDataService(context);
                var borrowingRepo = new BorrowingDataService(context);
                var config = new LibraryConfiguration();

                var readerService = new ReaderService(readerRepo);
                var bookService = new BookService(bookRepo, domainRepo, config);
                var borrowingService = new BorrowingService(borrowingRepo, bookRepo, readerRepo, config);

                // Setup: Create domain, book, reader
                var domain = new BookDomain { Name = "Return_" + Guid.NewGuid().ToString().Substring(0, 5) };
                context.Domains.Add(domain);
                context.SaveChanges();

                var createdDomain = context.Domains.FirstOrDefault(d => d.Name == domain.Name);

                var book = new Book
                {
                    Title = "Return Test Book",
                    ISBN = "77766655",
                    TotalCopies = 3,
                    ReadingRoomOnlyCopies = 0,
                    Authors = new List<Author> { new Author { FirstName = "Author", LastName = "Name" } }
                };
                bookService.CreateBook(book, new List<int> { createdDomain.Id });

                var createdBook = context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);

                var reader = new Reader
                {
                    FirstName = "Return",
                    LastName = "Tester",
                    Email = "return@" + Guid.NewGuid() + ".com",
                    Address = "Test Address"
                };
                readerService.CreateReader(reader);
                context.SaveChanges();

                var createdReader = context.Readers.FirstOrDefault(r => r.Email == reader.Email);

                // Borrow a book
                borrowingService.CreateBorrowings(createdReader.Id, new List<int> { createdBook.Id }, DateTime.Now, 14);

                var bookAfterBorrow = context.Books.Find(createdBook.Id);
                Assert.AreEqual(2, bookAfterBorrow.GetAvailableCopies(), "After borrow, should have 2 copies");

                // Get the borrowing
                var borrowing = context.Borrowings.FirstOrDefault(b => b.BookId == createdBook.Id && b.ReaderId == createdReader.Id);
                Assert.IsNotNull(borrowing);

                // Return the book
                borrowingService.ReturnBorrowing(borrowing.Id, DateTime.Now.AddDays(5));

                var bookAfterReturn = context.Books.Find(createdBook.Id);
                Assert.AreEqual(3, bookAfterReturn.GetAvailableCopies(), "After return, should have 3 copies again");

                // Cleanup
                context.Borrowings.Remove(borrowing);
                context.SaveChanges();

                context.Readers.Remove(createdReader);
                context.SaveChanges();

                context.Books.Remove(createdBook);
                context.SaveChanges();

                context.Domains.Remove(createdDomain);
                context.SaveChanges();
            }
        }
    }
    
}