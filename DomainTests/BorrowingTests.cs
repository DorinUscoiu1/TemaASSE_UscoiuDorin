using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DomainTests
{
    /// <summary>
    /// Unit tests for the Borrowing model class.
    /// </summary>
    [TestClass]
    public class BorrowingTests
    {
        private Borrowing borrowing;

        /// <summary>
        /// Initializes the test by creating a new Borrowing instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            borrowing = new Borrowing();
        }

        /// <summary>
        /// Test 1: Verifies that a borrowing record can be created as active and can be marked as returned.
        /// </summary>
        [TestMethod]
        public void Borrowing_ActiveBorrowing_CanBeReturned()
        {
            // Arrange
            borrowing.Id = 1;
            borrowing.ReaderId = 1;
            borrowing.BookId = 1;
            borrowing.BorrowingDate = DateTime.Now;
            borrowing.DueDate = DateTime.Now.AddDays(14);
            borrowing.IsActive = true;
            borrowing.ReturnDate = null;
            borrowing.InitialBorrowingDays = 14;

            // Act
            borrowing.ReturnDate = DateTime.Now;
            borrowing.IsActive = false;

            // Assert
            Assert.IsFalse(borrowing.IsActive);
            Assert.IsNotNull(borrowing.ReturnDate);
            Assert.IsTrue(borrowing.ReturnDate <= DateTime.Now);
        }

        /// <summary>
        /// Test 2: Verifies that borrowing extension information is tracked correctly.
        /// </summary>
        [TestMethod]
        public void Borrowing_ExtensionTracking_UpdatesCorrectly()
        {
            // Arrange
            borrowing.Id = 1;
            borrowing.DueDate = DateTime.Now.AddDays(14);
            borrowing.TotalExtensionDays = 0;
            borrowing.IsActive = true;

            // Act - First extension
            borrowing.DueDate = borrowing.DueDate.AddDays(7);
            borrowing.TotalExtensionDays += 7;
            borrowing.LastExtensionDate = DateTime.Now;

            // Assert
            Assert.AreEqual(7, borrowing.TotalExtensionDays);
            Assert.IsNotNull(borrowing.LastExtensionDate);

            // Act - Second extension
            borrowing.DueDate = borrowing.DueDate.AddDays(7);
            borrowing.TotalExtensionDays += 7;
            borrowing.LastExtensionDate = DateTime.Now;

            // Assert
            Assert.AreEqual(14, borrowing.TotalExtensionDays);
        }

        /// <summary>
        /// Test 3: Verifies that an overdue borrowing can be identified.
        /// </summary>
        [TestMethod]
        public void Borrowing_Overdue_IsIdentifiedCorrectly()
        {
            // Arrange
            borrowing.Id = 1;
            borrowing.BorrowingDate = DateTime.Now.AddDays(-20);
            borrowing.DueDate = DateTime.Now.AddDays(-5); // 5 days overdue
            borrowing.IsActive = true;
            borrowing.ReturnDate = null;

            // Act
            bool isOverdue = borrowing.IsActive && borrowing.DueDate < DateTime.Now;

            // Assert
            Assert.IsTrue(isOverdue);
        }

        /// <summary>
        /// Test 4: Verifies that a returned borrowing is not overdue.
        /// </summary>
        [TestMethod]
        public void Borrowing_Returned_IsNotOverdue()
        {
            // Arrange
            borrowing.Id = 1;
            borrowing.BorrowingDate = DateTime.Now.AddDays(-10);
            borrowing.DueDate = DateTime.Now.AddDays(5);
            borrowing.ReturnDate = DateTime.Now; // Returned on time
            borrowing.IsActive = false;

            // Act
            bool isOverdue = borrowing.IsActive && borrowing.DueDate < DateTime.Now;

            // Assert
            Assert.IsFalse(isOverdue);
            Assert.IsNotNull(borrowing.ReturnDate);
        }

        /// <summary>
        /// Test 5: Verifies that a borrowing record maintains correct relationships with reader and book.
        /// </summary>
        [TestMethod]
        public void Borrowing_WithReaderAndBook_MaintainsCorrectRelationships()
        {
            // Arrange
            var reader = new Reader { Id = 1, FirstName = "John", LastName = "Doe" };
            var book = new Book { Id = 1, Title = "Test Book" };

            // Act
            borrowing.Id = 1;
            borrowing.Reader = reader;
            borrowing.ReaderId = reader.Id;
            borrowing.Book = book;
            borrowing.BookId = book.Id;
            borrowing.BorrowingDate = DateTime.Now;
            borrowing.DueDate = DateTime.Now.AddDays(14);
            borrowing.IsActive = true;

            // Assert
            Assert.AreEqual(reader.Id, borrowing.ReaderId);
            Assert.AreEqual(book.Id, borrowing.BookId);
            Assert.AreEqual(reader.FirstName, borrowing.Reader.FirstName);
            Assert.AreEqual(book.Title, borrowing.Book.Title);
        }

        /// <summary>
        /// Test 6: Verifies GetTotalExtensionDays returns TotalExtensionDays when set.
        /// </summary>
        [TestMethod]
        public void Borrowing_GetTotalExtensionDays_ReturnsTotalExtensionDaysWhenSet()
        {
            // Arrange
            borrowing.TotalExtensionDays = 14;

            // Act
            int result = borrowing.GetTotalExtensionDays();

            // Assert
            Assert.AreEqual(14, result);
        }

        /// <summary>
        /// Test 7: Verifies GetTotalExtensionDays computes from Extensions collection when TotalExtensionDays is 0.
        /// </summary>
        [TestMethod]
        public void Borrowing_GetTotalExtensionDays_ComputesFromExtensionsWhenTotalIsZero()
        {
            // Arrange
            borrowing.TotalExtensionDays = 0;
            var extension1 = new LoanExtension { Id = 1, ExtensionDays = 7 };
            var extension2 = new LoanExtension { Id = 2, ExtensionDays = 7 };
            borrowing.Extensions.Add(extension1);
            borrowing.Extensions.Add(extension2);

            // Act
            int result = borrowing.GetTotalExtensionDays();

            // Assert
            Assert.AreEqual(14, result);
        }

        /// <summary>
        /// Test 8: Verifies GetTotalExtensionDays returns 0 when no extensions exist.
        /// </summary>
        [TestMethod]
        public void Borrowing_GetTotalExtensionDays_ReturnsZeroWhenNoExtensions()
        {
            // Arrange
            borrowing.TotalExtensionDays = 0;

            // Act
            int result = borrowing.GetTotalExtensionDays();

            // Assert
            Assert.AreEqual(0, result);
        }

        /// <summary>
        /// Test 9: Verifies default borrowing initialization.
        /// </summary>
        [TestMethod]
        public void Borrowing_DefaultInitialization_HasCorrectDefaults()
        {
            // Arrange & Act
            var newBorrowing = new Borrowing();

            // Assert
            Assert.AreEqual(0, newBorrowing.Id);
            Assert.AreEqual(0, newBorrowing.ReaderId);
            Assert.AreEqual(0, newBorrowing.BookId);
            Assert.IsTrue(newBorrowing.IsActive);
            Assert.IsNull(newBorrowing.Reader);
            Assert.IsNull(newBorrowing.Book);
            Assert.IsNull(newBorrowing.ReturnDate);
            Assert.IsNotNull(newBorrowing.Extensions);
            Assert.AreEqual(0, newBorrowing.TotalExtensionDays);
        }
    }
}
