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
        /// Test 1: Verifies that an edition can be created with all required properties.
        /// </summary>
        [TestMethod]
        public void Edition_CreateWithAllProperties_StoresCorrectly()
        {
            // Arrange & Act
            edition.Id = 1;
            edition.BookId = 1;
            edition.Publisher = "O'Reilly Media";
            edition.Year = 2023;
            edition.EditionNumber = 3;
            edition.PageCount = 456;
            edition.BookType = "Hardcover";

            // Assert
            Assert.AreEqual(1, edition.Id);
            Assert.AreEqual(1, edition.BookId);
            Assert.AreEqual("O'Reilly Media", edition.Publisher);
            Assert.AreEqual(2023, edition.Year);
            Assert.AreEqual(3, edition.EditionNumber);
            Assert.AreEqual(456, edition.PageCount);
            Assert.AreEqual("Hardcover", edition.BookType);
        }

        /// <summary>
        /// Test 2: Verifies that different book types can be assigned to editions.
        /// </summary>
        [TestMethod]
        public void Edition_DifferentBookTypes_AreStoredCorrectly()
        {
            // Arrange
            var hardcoverEdition = new Edition { Id = 1, BookType = "Hardcover" };
            var paperbackEdition = new Edition { Id = 2, BookType = "Paperback" };
            var ebookEdition = new Edition { Id = 3, BookType = "E-book" };

            // Act & Assert
            Assert.AreEqual("Hardcover", hardcoverEdition.BookType);
            Assert.AreEqual("Paperback", paperbackEdition.BookType);
            Assert.AreEqual("E-book", ebookEdition.BookType);
        }

        /// <summary>
        /// Test 3: Verifies that multiple editions of the same book have different years and numbers.
        /// </summary>
        [TestMethod]
        public void Edition_MultipleEditions_HaveDifferentYearsAndNumbers()
        {
            // Arrange
            var edition1 = new Edition { Id = 1, BookId = 1, Year = 2019, EditionNumber = 1 };
            var edition2 = new Edition { Id = 2, BookId = 1, Year = 2021, EditionNumber = 2 };
            var edition3 = new Edition { Id = 3, BookId = 1, Year = 2023, EditionNumber = 3 };

            // Act & Assert
            Assert.AreEqual(1, edition1.BookId);
            Assert.AreEqual(1, edition2.BookId);
            Assert.AreEqual(1, edition3.BookId);

            Assert.AreEqual(2019, edition1.Year);
            Assert.AreEqual(2021, edition2.Year);
            Assert.AreEqual(2023, edition3.Year);

            Assert.AreEqual(1, edition1.EditionNumber);
            Assert.AreEqual(2, edition2.EditionNumber);
            Assert.AreEqual(3, edition3.EditionNumber);
        }

        /// <summary>
        /// Test 4: Verifies that an edition maintains correct relationship with a book.
        /// </summary>
        [TestMethod]
        public void Edition_WithBook_MaintainsCorrectRelationship()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Programming Concepts" };

            // Act
            edition.Id = 1;
            edition.BookId = book.Id;
            edition.Book = book;
            edition.Publisher = "Tech Press";
            edition.Year = 2022;

            // Assert
            Assert.AreEqual(book.Id, edition.BookId);
            Assert.IsNotNull(edition.Book);
            Assert.AreEqual("Programming Concepts", edition.Book.Title);
            Assert.AreEqual("Tech Press", edition.Publisher);
        }
    }
}
