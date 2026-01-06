using Domain.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DomainTests
{
    /// <summary>
    /// Unit tests for the Edition model class.
    /// </summary>
    [TestClass]
    public class EditionTests
    {
        private Edition edition;

        /// <summary>
        /// Initializes the test by creating a new Edition instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            edition = new Edition();
        }

        /// <summary>
        /// Test 1: Verifies that an edition can be created with complete information.
        /// </summary>
        [TestMethod]
        public void Edition_CreateWithCompleteInfo_StoresCorrectly()
        {
            // Arrange & Act
            edition.Id = 1;
            edition.BookId = 5;
            edition.Publisher = "O'Reilly";
            edition.Year = 2022;
            edition.EditionNumber = 3;
            edition.PageCount = 456;
            edition.BookType = "Hardcover";

            // Assert
            Assert.AreEqual(1, edition.Id);
            Assert.AreEqual(5, edition.BookId);
            Assert.AreEqual("O'Reilly", edition.Publisher);
            Assert.AreEqual(2022, edition.Year);
            Assert.AreEqual(3, edition.EditionNumber);
            Assert.AreEqual(456, edition.PageCount);
            Assert.AreEqual("Hardcover", edition.BookType);
        }
        [TestMethod]
        public void Edition_CurrentYear_IsValid()
        {
            var edition = new Edition { Year = DateTime.Now.Year };

            Assert.AreEqual(edition.Year, (DateTime.Now.Year));
        }
        /// <summary>
        /// Test 2: Verifies that an edition can be associated with a book.
        /// </summary>
        [TestMethod]
        public void Edition_WithBook_MaintainsRelationship()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Programming in C#" };
            edition.Book = book;
            edition.BookId = book.Id;

            // Act & Assert
            Assert.IsNotNull(edition.Book);
            Assert.AreEqual(book.Id, edition.BookId);
            Assert.AreEqual("Programming in C#", edition.Book.Title);
        }

        /// <summary>
        /// Test 3: Verifies default edition initialization.
        /// </summary>
        [TestMethod]
        public void Edition_DefaultInitialization_HasCorrectDefaults()
        {
            // Arrange & Act
            var newEdition = new Edition();

            // Assert
            Assert.AreEqual(0, newEdition.Id);
            Assert.AreEqual(0, newEdition.BookId);
            Assert.IsNull(newEdition.Book);
            Assert.AreEqual(string.Empty, newEdition.Publisher);
            Assert.AreEqual(0, newEdition.Year);
            Assert.AreEqual(0, newEdition.EditionNumber);
            Assert.AreEqual(0, newEdition.PageCount);
            Assert.AreEqual(string.Empty, newEdition.BookType);
        }

        /// <summary>
        /// Test 4: Verifies that multiple editions can have different publishers and years.
        /// </summary>
        [TestMethod]
        public void Edition_MultipleEditions_StoreDistinctValues()
        {
            // Arrange
            var edition1 = new Edition { Id = 1, Publisher = "Packt", Year = 2020, EditionNumber = 1, PageCount = 400 };
            var edition2 = new Edition { Id = 2, Publisher = "Microsoft Press", Year = 2023, EditionNumber = 2, PageCount = 550 };

            // Act & Assert
            Assert.AreNotEqual(edition1.Publisher, edition2.Publisher);
            Assert.AreNotEqual(edition1.Year, edition2.Year);
            Assert.AreNotEqual(edition1.PageCount, edition2.PageCount);
        }

        /// <summary>
        /// Test book reference.
        /// </summary>
        [TestMethod]
        public void Edition_BookReference_CanBeSet()
        {
            var book = new Book { Id = 1 };
            var edition = new Edition { BookId = 1, Book = book };

            Assert.AreEqual(edition.Book, book);
        }
    }
}
