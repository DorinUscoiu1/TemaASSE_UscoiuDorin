using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Policy;

namespace DomainTests
{
    /// <summary>
    /// Unit tests for the Author model class.
    /// </summary>
    [TestClass]
    public class AuthorTests
    {
        private Author author;

        /// <summary>
        /// Initializes the test by creating a new Author instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            author = new Author();
        }

        /// <summary>
        /// Test 1: Verifies that an author can be created with basic information.
        /// </summary>
        [TestMethod]
        public void Author_CreateWithBasicInfo_StoresCorrectly()
        {
            // Arrange & Act
            author.Id = 1;
            author.FirstName = "Albert";
            author.LastName = "Einstein";

            // Assert
            Assert.AreEqual(1, author.Id);
            Assert.AreEqual("Albert", author.FirstName);
            Assert.AreEqual("Einstein", author.LastName);
        }

        /// <summary>
        /// Test 2: Verifies that GetFullName returns correct formatted name.
        /// </summary>
        [TestMethod]
        public void Author_GetFullName_ReturnsCorrectFormat()
        {
            // Arrange
            author.FirstName = "Isaac";
            author.LastName = "Newton";

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual("Isaac Newton", fullName);
        }

        /// <summary>
        /// Test with hyphenated names.
        /// </summary>
        [TestMethod]
        public void Author_HyphenatedNames_AreAccepted()
        {
            var author = new Author
            {
                FirstName = "Jean-Pierre",
                LastName = "Saint-Exupéry",
            };

            Assert.AreEqual(author.GetFullName(), "Jean-Pierre Saint-Exupéry");
        }
        /// <summary>
        /// Test with long names.
        /// </summary>
        [TestMethod]
        public void Author_LongNames_AreAccepted()
        {
            var longName = new string('A', 50);
            var author = new Author
            {
                FirstName = longName,
                LastName = longName,
            };

            Assert.AreEqual(author.FirstName.Length, 50);
        }

        /// <summary>
        /// Test 3: Verifies that books can be associated with an author.
        /// </summary>
        [TestMethod]
        public void Author_WithBooks_StoresCorrectly()
        {
            // Arrange
            author.Id = 1;
            author.FirstName = "Stephen";
            author.LastName = "Hawking";

            var book1 = new Book { Id = 1, Title = "A Brief History of Time" };
            var book2 = new Book { Id = 2, Title = "The Universe in a Nutshell" };

            // Act
            author.Books.Add(book1);
            author.Books.Add(book2);

            // Assert
            Assert.AreEqual(2, author.Books.Count);
            Assert.IsTrue(author.Books.Contains(book1));
            Assert.IsTrue(author.Books.Contains(book2));
        }

        /// <summary>
        /// Test 4: Verifies that GetFullName handles empty names.
        /// </summary>
        [TestMethod]
        public void Author_GetFullName_WithEmptyNames_ReturnsEmptyResult()
        {
            // Arrange
            author.FirstName = string.Empty;
            author.LastName = string.Empty;

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual(" ", fullName);
        }

        /// <summary>
        /// Test 5: Verifies default author initialization.
        /// </summary>
        [TestMethod]
        public void Author_DefaultInitialization_HasCorrectDefaults()
        {
            // Arrange & Act
            var newAuthor = new Author();

            // Assert
            Assert.AreEqual(0, newAuthor.Id);
            Assert.AreEqual(string.Empty, newAuthor.FirstName);
            Assert.AreEqual(string.Empty, newAuthor.LastName);
            Assert.IsNotNull(newAuthor.Books);
            Assert.AreEqual(0, newAuthor.Books.Count);
        }
    }
}
