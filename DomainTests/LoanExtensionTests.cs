// <copyright file="LoanExtensionTests.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace DomainTests
{
    using Domain.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// Unit tests for the LoanExtension model class.
    /// </summary>
    [TestClass]
    public class LoanExtensionTests
    {
        private LoanExtension loanExtension;

        /// <summary>
        /// Initializes the test by creating a new LoanExtension instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            loanExtension = new LoanExtension();
        }

        /// <summary>
        /// Test 1: LoanExtension can be created with basic information.
        /// </summary>
        [TestMethod]
        public void LoanExtension_CreateWithBasicInfo_StoresCorrectly()
        {
            // Arrange & Act
            loanExtension.Id = 1;
            loanExtension.BorrowingId = 1;
            loanExtension.ExtensionDays = 7;
            loanExtension.ExtensionDate = DateTime.Now;

            // Assert
            Assert.AreEqual(1, loanExtension.Id);
            Assert.AreEqual(1, loanExtension.BorrowingId);
            Assert.AreEqual(7, loanExtension.ExtensionDays);
            Assert.IsNotNull(loanExtension.ExtensionDate);
        }

        /// <summary>
        /// Test 2: LoanExtension with borrowing reference.
        /// </summary>
        [TestMethod]
        public void LoanExtension_WithBorrowingReference_MaintainsCorrectRelationship()
        {
            // Arrange
            var borrowing = new Borrowing { Id = 1, BorrowingDate = DateTime.Now, DueDate = DateTime.Now.AddDays(14) };
            loanExtension.Borrowing = borrowing;
            loanExtension.BorrowingId = 1;

            // Act & Assert
            Assert.IsNotNull(loanExtension.Borrowing);
            Assert.AreEqual(1, loanExtension.BorrowingId);
            Assert.AreEqual(borrowing.Id, loanExtension.Borrowing.Id);
        }

        /// <summary>
        /// Test 3: Multiple extensions for same borrowing.
        /// </summary>
        [TestMethod]
        public void LoanExtension_MultipleExtensionsForSameBorrowing_WorkCorrectly()
        {
            // Arrange
            var borrowing = new Borrowing { Id = 1 };
            var extension1 = new LoanExtension { Id = 1, BorrowingId = 1, Borrowing = borrowing, ExtensionDays = 7 };
            var extension2 = new LoanExtension { Id = 2, BorrowingId = 1, Borrowing = borrowing, ExtensionDays = 7 };

            // Act
            borrowing.Extensions.Add(extension1);
            borrowing.Extensions.Add(extension2);

            // Assert
            Assert.AreEqual(2, borrowing.Extensions.Count);
            Assert.AreEqual(7, extension1.ExtensionDays);
            Assert.AreEqual(7, extension2.ExtensionDays);
        }

        /// <summary>
        /// Test 4: Extension days can vary.
        /// </summary>
        [TestMethod]
        public void LoanExtension_WithVaryingDays_StoresCorrectly()
        {
            // Arrange
            var ext1 = new LoanExtension { Id = 1, ExtensionDays = 3 };
            var ext2 = new LoanExtension { Id = 2, ExtensionDays = 7 };
            var ext3 = new LoanExtension { Id = 3, ExtensionDays = 14 };
            var ext4 = new LoanExtension { Id = 4, ExtensionDays = 30 };

            // Act & Assert
            Assert.AreEqual(3, ext1.ExtensionDays);
            Assert.AreEqual(7, ext2.ExtensionDays);
            Assert.AreEqual(14, ext3.ExtensionDays);
            Assert.AreEqual(30, ext4.ExtensionDays);
        }

        /// <summary>
        /// Test 5: Extension date tracks when extension was granted.
        /// </summary>
        [TestMethod]
        public void LoanExtension_ExtensionDate_TracksGrantDate()
        {
            // Arrange
            var now = DateTime.Now;
            loanExtension.ExtensionDate = now;

            // Act & Assert
            Assert.AreEqual(now.Date, loanExtension.ExtensionDate.Date);
        }

        /// <summary>
        /// Test 6: Extension with past date.
        /// </summary>
        [TestMethod]
        public void LoanExtension_WithPastDate_StoresCorrectly()
        {
            // Arrange
            var pastDate = DateTime.Now.AddDays(-7);
            loanExtension.ExtensionDate = pastDate;

            // Act & Assert
            Assert.IsTrue(loanExtension.ExtensionDate < DateTime.Now);
            Assert.AreEqual(pastDate.Date, loanExtension.ExtensionDate.Date);
        }

        /// <summary>
        /// Test 7: Default loan extension initialization.
        /// </summary>
        [TestMethod]
        public void LoanExtension_DefaultInitialization_HasCorrectDefaults()
        {
            // Arrange & Act
            var newExtension = new LoanExtension();

            // Assert
            Assert.AreEqual(0, newExtension.Id);
            Assert.AreEqual(0, newExtension.BorrowingId);
            Assert.AreEqual(0, newExtension.ExtensionDays);
            Assert.IsNull(newExtension.Borrowing);
        }

        /// <summary>
        /// Test 8: Total extension days accumulation.
        /// </summary>
        [TestMethod]
        public void LoanExtension_MultipleExtensionsDaysAccumulate_CalculatesCorrectly()
        {
            // Arrange
            var extension1 = new LoanExtension { Id = 1, ExtensionDays = 7 };
            var extension2 = new LoanExtension { Id = 2, ExtensionDays = 7 };
            var extension3 = new LoanExtension { Id = 3, ExtensionDays = 7 };

            // Act
            int totalDays = extension1.ExtensionDays + extension2.ExtensionDays + extension3.ExtensionDays;

            // Assert
            Assert.AreEqual(21, totalDays);
        }

        /// <summary>
        /// Test 9: Extension with zero days edge case.
        /// </summary>
        [TestMethod]
        public void LoanExtension_WithZeroDays_StoresCorrectly()
        {
            // Arrange
            loanExtension.ExtensionDays = 0;

            // Act & Assert
            Assert.AreEqual(0, loanExtension.ExtensionDays);
        }

        /// <summary>
        /// Test 10: Extension with maximum reasonable days.
        /// </summary>
        [TestMethod]
        public void LoanExtension_WithMaximumDays_StoresCorrectly()
        {
            // Arrange
            loanExtension.ExtensionDays = 365; // One year max

            // Act & Assert
            Assert.AreEqual(365, loanExtension.ExtensionDays);
        }
    }
}