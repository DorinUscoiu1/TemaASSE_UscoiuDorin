using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainTests
{
    /// <summary>
    /// Unit tests for the BookDomain model class.
    /// </summary>
    [TestClass]
    public class BookDomainTests
    {
        private BookDomain domain;

        /// <summary>
        /// Initializes the test by creating a new BookDomain instance before each test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            domain = new BookDomain();
        }

        /// <summary>
        /// Test 1: Verifies that a BookDomain can be created with a parent domain forming a hierarchical structure.
        /// </summary>
        [TestMethod]
        public void BookDomain_WithParentDomain_CreatesHierarchicalStructure()
        {
            // Arrange
            var parentDomain = new BookDomain { Id = 1, Name = "Science" };
            var subDomain = new BookDomain { Id = 2, Name = "Physics", ParentDomainId = 1 };

            // Act
            subDomain.ParentDomain = parentDomain;
            parentDomain.Subdomains.Add(subDomain);

            // Assert
            Assert.IsNotNull(subDomain.ParentDomain);
            Assert.AreEqual(parentDomain.Id, subDomain.ParentDomain.Id);
            Assert.AreEqual("Science", subDomain.ParentDomain.Name);
            Assert.IsTrue(parentDomain.Subdomains.Contains(subDomain));
            Assert.AreEqual(1, parentDomain.Subdomains.Count);
        }

        /// <summary>
        /// Test 2: Verifies that books can be added to a domain and retrieved correctly.
        /// </summary>
        [TestMethod]
        public void BookDomain_AddBooks_StoresBooksCorrectly()
        {
            // Arrange
            domain.Id = 1;
            domain.Name = "Mathematics";
            var book1 = new Book { Id = 1, Title = "Algebra" };
            var book2 = new Book { Id = 2, Title = "Geometry" };

            // Act
            domain.Books.Add(book1);
            domain.Books.Add(book2);

            // Assert
            Assert.AreEqual(2, domain.Books.Count);
            Assert.IsTrue(domain.Books.Contains(book1));
            Assert.IsTrue(domain.Books.Contains(book2));
            Assert.IsTrue(domain.Books.Any(b => b.Title == "Algebra"));
            Assert.IsTrue(domain.Books.Any(b => b.Title == "Geometry"));
        }

        /// <summary>
        /// Test 3: Verifies that a root domain (without parent) is created correctly.
        /// </summary>
        [TestMethod]
        public void BookDomain_RootDomain_HasNoParent()
        {
            // Arrange & Act
            domain.Id = 1;
            domain.Name = "Root Domain";
            domain.ParentDomainId = null;

            // Assert
            Assert.IsNull(domain.ParentDomainId);
            Assert.IsNull(domain.ParentDomain);
            Assert.AreEqual("Root Domain", domain.Name);
        }

        /// <summary>
        /// Test 4: Verifies that multiple subdomains can be added to a parent domain.
        /// </summary>
        [TestMethod]
        public void BookDomain_MultipleSubdomains_AreStoredCorrectly()
        {
            // Arrange
            var parentDomain = new BookDomain { Id = 1, Name = "Science" };
            var subDomain1 = new BookDomain { Id = 2, Name = "Physics", ParentDomainId = 1 };
            var subDomain2 = new BookDomain { Id = 3, Name = "Chemistry", ParentDomainId = 1 };
            var subDomain3 = new BookDomain { Id = 4, Name = "Biology", ParentDomainId = 1 };

            // Act
            parentDomain.Subdomains.Add(subDomain1);
            parentDomain.Subdomains.Add(subDomain2);
            parentDomain.Subdomains.Add(subDomain3);

            // Assert
            Assert.AreEqual(3, parentDomain.Subdomains.Count);
            Assert.IsTrue(parentDomain.Subdomains.All(s => s.ParentDomainId == 1));
        }
    }
}
