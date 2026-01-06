using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DomainTests
{
    /// <summary>
    /// Unit tests for the LoanExtension model class.
    /// </summary>
    [TestClass]
    public class LoanExtensionTests
    {
        private LoanExtension extension;

        /// <summary>
        /// Initializes the test by creating a new LoanExtension instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            extension = new LoanExtension();
        }

        /// <summary>
        /// Test 1: Verifies that a loan extension can be created with complete information.
        /// </summary>
        [TestMethod]
        public void LoanExtension_CreateWithCompleteInfo_StoresCorrectly()
        {
            // Arrange & Act
            extension.Id = 1;
            extension.BorrowingId = 5;
            extension.ExtensionDate = DateTime.Now;
            extension.ExtensionDays = 7;

            // Assert
            Assert.AreEqual(1, extension.Id);
            Assert.AreEqual(5, extension.BorrowingId);
            Assert.AreEqual(7, extension.ExtensionDays);
            Assert.IsNotNull(extension.ExtensionDate);
        }

        /// <summary>
        /// Test 2: Verifies that a loan extension can be associated with a borrowing.
        /// </summary>
        [TestMethod]
        public void LoanExtension_WithBorrowing_MaintainsRelationship()
        {
            // Arrange
            var borrowing = new Borrowing { Id = 1, ReaderId = 1, BookId = 1 };
            extension.Borrowing = borrowing;
            extension.BorrowingId = borrowing.Id;

            // Act & Assert
            Assert.IsNotNull(extension.Borrowing);
            Assert.AreEqual(borrowing.Id, extension.BorrowingId);
        }

        /// <summary>
        /// Test 3: Verifies default loan extension initialization.
        /// </summary>
        [TestMethod]
        public void LoanExtension_DefaultInitialization_HasCorrectDefaults()
        {
            // Arrange & Act
            var newExtension = new LoanExtension();

            // Assert
            Assert.AreEqual(0, newExtension.Id);
            Assert.AreEqual(0, newExtension.BorrowingId);
            Assert.IsNull(newExtension.Borrowing);
            Assert.AreEqual(0, newExtension.ExtensionDays);
        }

        /// <summary>
        /// Test 4: Verifies that multiple extensions can be tracked.
        /// </summary>
        [TestMethod]
        public void LoanExtension_MultipleExtensions_TrackCorrectly()
        {
            // Arrange
            var borrowing = new Borrowing { Id = 1 };

            var ext1 = new LoanExtension { Id = 1, BorrowingId = 1, ExtensionDays = 7, ExtensionDate = DateTime.Now };
            var ext2 = new LoanExtension { Id = 2, BorrowingId = 1, ExtensionDays = 7, ExtensionDate = DateTime.Now.AddDays(7) };

            borrowing.Extensions.Add(ext1);
            borrowing.Extensions.Add(ext2);

            // Act & Assert
            Assert.AreEqual(2, borrowing.Extensions.Count);
            Assert.AreEqual(14, ext1.ExtensionDays + ext2.ExtensionDays);
        }

        /// <summary>
        /// Test extension date in past.
        /// </summary>
        [TestMethod]
        public void LoanExtension_PastDate_IsValid()
        {
            var extension = new LoanExtension
            {
                ExtensionDate = DateTime.Now.AddDays(-10),
            };

            Assert.IsTrue(extension.ExtensionDate<DateTime.Now);
        }
    }
}