using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DomainTests
{
    /// <summary>
    /// Unit tests for the Reader model class.
    /// </summary>
    [TestClass]
    public class ReaderTests
    {
        private Reader reader;

        /// <summary>
        /// Initializes the test by creating a new Reader instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            reader = new Reader();
        }

        /// <summary>
        /// Test 1: Verifies that a reader can be created with basic information.
        /// </summary>
        [TestMethod]
        public void Reader_CreateWithBasicInfo_StoresCorrectly()
        {
            // Arrange & Act
            reader.Id = 1;
            reader.FirstName = "John";
            reader.LastName = "Doe";
            reader.Email = "john@example.com";
            reader.Address = "123 Main Street";
            reader.PhoneNumber = "555-1234";
            reader.IsStaff = false;
            reader.RegistrationDate = DateTime.Now;

            // Assert
            Assert.AreEqual(1, reader.Id);
            Assert.AreEqual("John", reader.FirstName);
            Assert.AreEqual("Doe", reader.LastName);
            Assert.AreEqual("john@example.com", reader.Email);
            Assert.IsFalse(reader.IsStaff);
        }

        /// <summary>
        /// Test 2: Verifies that staff and non-staff readers can be distinguished.
        /// </summary>
        [TestMethod]
        public void Reader_StaffVsNonStaff_IsIdentifiedCorrectly()
        {
            // Arrange
            var staffReader = new Reader { Id = 1, FirstName = "Alice", IsStaff = true };
            var regularReader = new Reader { Id = 2, FirstName = "Bob", IsStaff = false };

            // Act & Assert
            Assert.IsTrue(staffReader.IsStaff);
            Assert.IsFalse(regularReader.IsStaff);
        }

        /// <summary>
        /// Test 3: Verifies that reader borrowing records are tracked correctly.
        /// </summary>
        [TestMethod]
        public void Reader_BorrowingRecords_AreTrackedCorrectly()
        {
            // Arrange
            reader.Id = 1;
            reader.FirstName = "Jane";

            var borrowing1 = new Borrowing { Id = 1, ReaderId = 1, BookId = 1, IsActive = true, ReturnDate = null };
            var borrowing2 = new Borrowing { Id = 2, ReaderId = 1, BookId = 2, IsActive = true, ReturnDate = null };
            var borrowing3 = new Borrowing { Id = 3, ReaderId = 1, BookId = 3, IsActive = false, ReturnDate = DateTime.Now };

            // Act
            reader.BorrowingRecords.Add(borrowing1);
            reader.BorrowingRecords.Add(borrowing2);
            reader.BorrowingRecords.Add(borrowing3);

            int activeBorrowings = reader.BorrowingRecords.Count(b => b.IsActive);

            // Assert
            Assert.AreEqual(3, reader.BorrowingRecords.Count);
            Assert.AreEqual(2, activeBorrowings);
        }

        /// <summary>
        /// Test 4: Verifies that registration date is tracked for a reader.
        /// </summary>
        [TestMethod]
        public void Reader_RegistrationDate_IsTrackedCorrectly()
        {
            // Arrange
            var registrationDate = new DateTime(2023, 6, 15);

            // Act
            reader.RegistrationDate = registrationDate;

            // Assert
            Assert.AreEqual(2023, reader.RegistrationDate.Year);
            Assert.AreEqual(6, reader.RegistrationDate.Month);
            Assert.AreEqual(15, reader.RegistrationDate.Day);
        }

        /// <summary>
        /// Test 5: Verifies that GetFullName returns correct formatted name.
        /// </summary>
        [TestMethod]
        public void Reader_GetFullName_ReturnsCorrectFormat()
        {
            // Arrange
            reader.FirstName = "John";
            reader.LastName = "Doe";

            // Act
            string fullName = reader.GetFullName();

            // Assert
            Assert.AreEqual("John Doe", fullName);
        }

        /// <summary>
        /// Test 6: Verifies that GetFullName handles empty names.
        /// </summary>
        [TestMethod]
        public void Reader_GetFullName_WithEmptyNames_ReturnsEmptyResult()
        {
            // Arrange
            reader.FirstName = string.Empty;
            reader.LastName = string.Empty;

            // Act
            string fullName = reader.GetFullName();

            // Assert
            Assert.AreEqual(" ", fullName);
        }

        /// <summary>
        /// Test 7: Verifies default property initialization.
        /// </summary>
        [TestMethod]
        public void Reader_DefaultInitialization_HasEmptyCollections()
        {
            // Arrange & Act
            var newReader = new Reader();

            // Assert
            Assert.IsNotNull(newReader.BorrowingRecords);
            Assert.AreEqual(0, newReader.BorrowingRecords.Count);
            Assert.AreEqual(string.Empty, newReader.FirstName);
            Assert.AreEqual(string.Empty, newReader.LastName);
            Assert.AreEqual(string.Empty, newReader.Address);
            Assert.AreEqual(string.Empty, newReader.PhoneNumber);
            Assert.AreEqual(string.Empty, newReader.Email);
        }
    }
}
