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
        /// Test child is not ancestor of parent.
        /// </summary>
        [TestMethod]
        public void IsAncestorOf_ChildToParent_ReturnsFalse()
        {
            var parent = new BookDomain { Id = 1, Name = "Science" };
            var child = new BookDomain { Id = 2, Name = "Computer Science", ParentDomain = parent };

            Assert.AreEqual(child.IsAncestorOf(parent), false);
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

        /// <summary>
        /// Test 5: Verifies that GetAncestors returns complete ancestor chain.
        /// </summary>
        [TestMethod]
        public void BookDomain_GetAncestors_ReturnsCompleteChain()
        {
            // Arrange
            var root = new BookDomain { Id = 1, Name = "Root" };
            var middle = new BookDomain { Id = 2, Name = "Middle", ParentDomain = root, ParentDomainId = 1 };
            var leaf = new BookDomain { Id = 3, Name = "Leaf", ParentDomain = middle, ParentDomainId = 2 };

            // Act
            var ancestors = leaf.GetAncestors();

            // Assert
            Assert.AreEqual(3, ancestors.Count);
            Assert.AreEqual(leaf.Id, ancestors[0].Id);
            Assert.AreEqual(middle.Id, ancestors[1].Id);
            Assert.AreEqual(root.Id, ancestors[2].Id);
        }

        /// <summary>
        /// Test 6: Verifies that IsAncestorOf returns true when domain is ancestor.
        /// </summary>
        [TestMethod]
        public void BookDomain_IsAncestorOf_ReturnsTrueWhenAncestor()
        {
            // Arrange
            var root = new BookDomain { Id = 1, Name = "Root" };
            var middle = new BookDomain { Id = 2, Name = "Middle", ParentDomain = root, ParentDomainId = 1 };
            var leaf = new BookDomain { Id = 3, Name = "Leaf", ParentDomain = middle, ParentDomainId = 2 };

            // Act
            bool isAncestor = root.IsAncestorOf(leaf);

            // Assert
            Assert.IsTrue(isAncestor);
        }

        /// <summary>
        /// Test 7: Verifies that IsAncestorOf returns false when not ancestor.
        /// </summary>
        [TestMethod]
        public void BookDomain_IsAncestorOf_ReturnsFalseWhenNotAncestor()
        {
            // Arrange
            var domain1 = new BookDomain { Id = 1, Name = "Domain1" };
            var domain2 = new BookDomain { Id = 2, Name = "Domain2" };

            // Act
            bool isAncestor = domain1.IsAncestorOf(domain2);

            // Assert
            Assert.IsFalse(isAncestor);
        }

        /// <summary>
        /// Test 8: Verifies that GetAncestors on root returns only root.
        /// </summary>
        [TestMethod]
        public void BookDomain_GetAncestors_RootReturnsOnlyItself()
        {
            // Arrange
            var root = new BookDomain { Id = 1, Name = "Root" };

            // Act
            var ancestors = root.GetAncestors();

            // Assert
            Assert.AreEqual(1, ancestors.Count);
            Assert.AreEqual(root.Id, ancestors[0].Id);
        }

        /// <summary>
        /// Test 9: Verifies default domain initialization.
        /// </summary>
        [TestMethod]
        public void BookDomain_DefaultInitialization_HasCorrectDefaults()
        {
            // Arrange & Act
            var newDomain = new BookDomain();

            // Assert
            Assert.AreEqual(0, newDomain.Id);
            Assert.AreEqual(string.Empty, newDomain.Name);
            Assert.IsNull(newDomain.ParentDomainId);
            Assert.IsNull(newDomain.ParentDomain);
            Assert.IsNotNull(newDomain.Subdomains);
            Assert.IsNotNull(newDomain.Books);
            Assert.AreEqual(0, newDomain.Subdomains.Count);
            Assert.AreEqual(0, newDomain.Books.Count);
        }
    }
}
