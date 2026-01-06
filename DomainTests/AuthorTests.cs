// <copyright file="AuthorTests.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace DomainTests
{
    using Domain.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        /// Test 1: Author can be created with basic information.
        /// </summary>
        [TestMethod]
        public void Author_CreateWithBasicInfo_StoresCorrectly()
        {
            // Arrange & Act
            author.Id = 1;
            author.FirstName = "Stephen";
            author.LastName = "King";

            // Assert
            Assert.AreEqual(1, author.Id);
            Assert.AreEqual("Stephen", author.FirstName);
            Assert.AreEqual("King", author.LastName);
        }

        /// <summary>
        /// Test 2: GetFullName returns correct formatted name.
        /// </summary>
        [TestMethod]
        public void Author_GetFullName_ReturnsCorrectFormat()
        {
            // Arrange
            author.FirstName = "George";
            author.LastName = "Martin";

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual("George Martin", fullName);
        }

        /// <summary>
        /// Test 3: GetFullName with empty first name.
        /// </summary>
        [TestMethod]
        public void Author_GetFullName_WithEmptyFirstName_ReturnsLastNameOnly()
        {
            // Arrange
            author.FirstName = string.Empty;
            author.LastName = "Tolkien";

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual(" Tolkien", fullName);
        }

        /// <summary>
        /// Test 4: GetFullName with empty last name.
        /// </summary>
        [TestMethod]
        public void Author_GetFullName_WithEmptyLastName_ReturnsFirstNameOnly()
        {
            // Arrange
            author.FirstName = "Isaac";
            author.LastName = string.Empty;

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual("Isaac ", fullName);
        }

        /// <summary>
        /// Test 5: Author can have multiple books.
        /// </summary>
        [TestMethod]
        public void Author_WithMultipleBooks_StoresCorrectly()
        {
            // Arrange
            author.Id = 1;
            author.FirstName = "J.K.";
            author.LastName = "Rowling";

            var book1 = new Book { Id = 1, Title = "Harry Potter 1" };
            var book2 = new Book { Id = 2, Title = "Harry Potter 2" };
            var book3 = new Book { Id = 3, Title = "Harry Potter 3" };

            // Act
            author.Books.Add(book1);
            author.Books.Add(book2);
            author.Books.Add(book3);

            // Assert
            Assert.AreEqual(3, author.Books.Count);
            Assert.IsTrue(author.Books.Contains(book1));
            Assert.IsTrue(author.Books.Contains(book2));
            Assert.IsTrue(author.Books.Contains(book3));
        }

        /// <summary>
        /// Test 6: Default author initialization has empty collections.
        /// </summary>
        [TestMethod]
        public void Author_DefaultInitialization_HasEmptyCollections()
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

        /// <summary>
        /// Test 7: Author with whitespace names.
        /// </summary>
        [TestMethod]
        public void Author_WithWhitespaceNames_StoresAsIs()
        {
            // Arrange
            author.FirstName = "   John   ";
            author.LastName = "   Doe   ";

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual("   John       Doe   ", fullName);
        }

        /// <summary>
        /// Test 8: Author with special characters in names.
        /// </summary>
        [TestMethod]
        public void Author_WithSpecialCharacters_StoresCorrectly()
        {
            // Arrange
            author.FirstName = "José";
            author.LastName = "García";

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual("José García", fullName);
        }

        /// <summary>
        /// Test 9: Author with very long names.
        /// </summary>
        [TestMethod]
        public void Author_WithLongNames_StoresCorrectly()
        {
            // Arrange
            author.FirstName = "Christopher";
            author.LastName = "Stracheyewskimilosolevich";

            // Act
            string fullName = author.GetFullName();

            // Assert
            Assert.AreEqual("Christopher Stracheyewskimilosolevich", fullName);
            Assert.IsTrue(fullName.Length > 30);
        }

        /// <summary>
        /// Test 10: Multiple authors can share the same book.
        /// </summary>
        [TestMethod]
        public void Author_MultipleAuthorsWithSharedBook_WorksCorrectly()
        {
            // Arrange
            var author1 = new Author { Id = 1, FirstName = "Neil", LastName = "Gaiman" };
            var author2 = new Author { Id = 2, FirstName = "Terry", LastName = "Pratchett" };
            var sharedBook = new Book { Id = 1, Title = "Good Omens" };

            // Act
            author1.Books.Add(sharedBook);
            author2.Books.Add(sharedBook);

            // Assert
            Assert.AreEqual(1, author1.Books.Count);
            Assert.AreEqual(1, author2.Books.Count);
            Assert.AreEqual(sharedBook.Title, author1.Books.First().Title);
            Assert.AreEqual(sharedBook.Title, author2.Books.First().Title);
        }
    }
}
