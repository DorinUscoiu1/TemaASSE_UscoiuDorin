using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DomainTests
{
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
        /// Tests that a new Author instance is created with default values.
        /// </summary>
        [TestMethod]
        public void AuthorConstructor_CreatesNewInstance_WithDefaultValues()
        {
            var newAuthor = new Author();
            Assert.IsNotNull(newAuthor);
            Assert.AreEqual(0, newAuthor.Id);
            Assert.IsNull(newAuthor.FirstName);
            Assert.IsNull(newAuthor.LastName);
        }

        [TestMethod]
        public void Author_Books_IsInitialized()
        {
            var author = new Author();

            Assert.IsNotNull(author.Books);
            Assert.AreEqual(0, author.Books.Count);
        }
    }
}
