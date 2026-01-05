using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainTests
{
    /// <summary>
    /// Unit tests for the Book model class.
    /// </summary>
    [TestClass]
    public class BookTests
    {
        private Book book;

        /// <summary>
        /// Initializes the test by creating a new Book instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            book = new Book();
        }

        /// <summary>
        /// Test 1: Verifies that GetAvailableCopies calculates the correct number of available copies.
        /// Available = TotalCopies - ReadingRoomOnlyCopies - LoanedCopies
        /// </summary>
        [TestMethod]
        public void Book_GetAvailableCopies_CalculatesCorrectly()
        {
            // Arrange
            book.Id = 1;
            book.Title = "Test Book";
            book.TotalCopies = 10;
            book.ReadingRoomOnlyCopies = 2;
            
            var activeBorrowing1 = new Borrowing { Id = 1, ReturnDate = null }; // Not returned
            var activeBorrowing2 = new Borrowing { Id = 2, ReturnDate = null }; // Not returned
            var returnedBorrowing = new Borrowing { Id = 3, ReturnDate = DateTime.Now }; // Returned

            book.BorrowingRecords.Add(activeBorrowing1);
            book.BorrowingRecords.Add(activeBorrowing2);
            book.BorrowingRecords.Add(returnedBorrowing);

            // Act
            int availableCopies = book.GetAvailableCopies();

            // Assert
            // Available = 10 - 2 - 2 = 6
            Assert.AreEqual(6, availableCopies);
        }

        /// <summary>
        /// Test 2: Verifies that a book can be associated with multiple authors and domains.
        /// </summary>
        [TestMethod]
        public void Book_WithMultipleAuthorsAndDomains_StoresCorrectly()
        {
            // Arrange
            book.Id = 1;
            book.Title = "Advanced Physics";
            book.ISBN = "ISBN-12345";

            var author1 = new Author { Id = 1, FirstName = "Albert", LastName = "Einstein" };
            var author2 = new Author { Id = 2, FirstName = "Niels", LastName = "Bohr" };
            
            var domain1 = new BookDomain { Id = 1, Name = "Physics" };
            var domain2 = new BookDomain { Id = 2, Name = "Science" };

            // Act
            book.Authors.Add(author1);
            book.Authors.Add(author2);
            book.Domains.Add(domain1);
            book.Domains.Add(domain2);

            // Assert
            Assert.AreEqual(2, book.Authors.Count);
            Assert.AreEqual(2, book.Domains.Count);
            Assert.IsTrue(book.Authors.Any(a => a.FirstName == "Albert"));
            Assert.IsTrue(book.Domains.Any(d => d.Name == "Physics"));
        }

        /// <summary>
        /// Test 3: Verifies that GetAvailableCopies returns 0 when all copies are loaned or reading-room-only.
        /// </summary>
        [TestMethod]
        public void Book_GetAvailableCopies_ReturnsZeroWhenNoAvailableCopies()
        {
            // Arrange
            book.TotalCopies = 5;
            book.ReadingRoomOnlyCopies = 2;
            
            // Add 3 active borrowings (all copies are loaned)
            for (int i = 1; i <= 3; i++)
            {
                book.BorrowingRecords.Add(new Borrowing { Id = i, ReturnDate = null });
            }

            // Act
            int availableCopies = book.GetAvailableCopies();

            // Assert
            // Available = 5 - 2 - 3 = 0
            Assert.AreEqual(0, availableCopies);
        }

        /// <summary>
        /// Test 4: Verifies that editions can be added to a book correctly.
        /// </summary>
        [TestMethod]
        public void Book_WithEditions_StoresEditionsCorrectly()
        {
            // Arrange
            book.Id = 1;
            book.Title = "Programming Languages";

            var edition1 = new Edition { Id = 1, BookId = 1, Publisher = "O'Reilly", Year = 2020 };
            var edition2 = new Edition { Id = 2, BookId = 1, Publisher = "O'Reilly", Year = 2022 };

            // Act
            book.Editions.Add(edition1);
            book.Editions.Add(edition2);

            // Assert
            Assert.AreEqual(2, book.Editions.Count);
            Assert.IsTrue(book.Editions.All(e => e.BookId == book.Id));
            Assert.IsTrue(book.Editions.Any(e => e.Year == 2020));
            Assert.IsTrue(book.Editions.Any(e => e.Year == 2022));
        }
    }
}
