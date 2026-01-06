// <copyright file="FluentValidationTests.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace ServiceTests
{
    using Data.Validators;
    using Domain.Models;
    using FluentValidation.TestHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Fluent validation tests using ShouldHaveValidationErrorFor and ShouldNotHaveValidationErrorFor.
    /// </summary>
    [TestClass]
    public class FluentValidationTests
    {
        #region BookValidator - ShouldNotHaveValidationErrorFor

        /// <summary>
        /// Test 8: BookValidator - Negative total copies should have validation error for TotalCopies.
        /// </summary>
        [TestMethod]
        public void BookValidator_NegativeTotalCopies_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new BookValidator();
            var book = new Book
            {
                Title = "Valid Title",
                ISBN = "1234567890",
                TotalCopies = -5,
                ReadingRoomOnlyCopies = 0
            };

            // Act & Assert
            var result = validator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(b => b.TotalCopies);
        }

        /// <summary>
        /// Test 9: BookValidator - Negative reading room copies should have validation error.
        /// </summary>
        [TestMethod]
        public void BookValidator_NegativeReadingRoomCopies_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new BookValidator();
            var book = new Book
            {
                Title = "Valid Title",
                ISBN = "1234567890",
                TotalCopies = 10,
                ReadingRoomOnlyCopies = -1
            };

            // Act & Assert
            var result = validator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(b => b.ReadingRoomOnlyCopies);
        }

        /// <summary>
        /// Test 10: BookValidator - Reading room exceeding total should have validation error.
        /// </summary>
        [TestMethod]
        public void BookValidator_ReadingRoomExceedingTotal_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new BookValidator();
            var book = new Book
            {
                Title = "Valid Title",
                ISBN = "1234567890",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = 10
            };

            // Act & Assert
            var result = validator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(b => b.ReadingRoomOnlyCopies);
        }

        /// <summary>
        /// Test 11: BookValidator - Null ISBN should have validation error for ISBN.
        /// </summary>
        [TestMethod]
        public void BookValidator_NullISBN_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new BookValidator();
            var book = new Book
            {
                Title = "Valid Title",
                ISBN = null,
                TotalCopies = 5,
                ReadingRoomOnlyCopies = 1
            };

            // Act & Assert
            var result = validator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(b => b.ISBN);
        }

        /// <summary>
        /// Test 12: BookValidator - Invalid ISBN format should have validation error for ISBN.
        /// </summary>
        [TestMethod]
        public void BookValidator_InvalidISBNFormat_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new BookValidator();
            var book = new Book
            {
                Title = "Valid Title",
                ISBN = "ABC",
                TotalCopies = 5,
                ReadingRoomOnlyCopies = 1
            };

            // Act & Assert
            var result = validator.TestValidate(book);
            result.ShouldHaveValidationErrorFor(b => b.ISBN);
        }

        #endregion

        #region AuthorValidator - ShouldNotHaveValidationErrorFor

        /// <summary>
        /// Test 13: AuthorValidator - Valid author should not have validation errors for FirstName.
        /// </summary>
        [TestMethod]
        public void AuthorValidator_ValidFirstName_ShouldNotHaveValidationErrorFor()
        {
            // Arrange
            var validator = new AuthorValidator();
            var author = new Author
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act & Assert
            var result = validator.TestValidate(author);
            result.ShouldNotHaveValidationErrorFor(a => a.FirstName);
        }

        /// <summary>
        /// Test 14: AuthorValidator - Valid author should not have validation errors for LastName.
        /// </summary>
        [TestMethod]
        public void AuthorValidator_ValidLastName_ShouldNotHaveValidationErrorFor()
        {
            // Arrange
            var validator = new AuthorValidator();
            var author = new Author
            {
                FirstName = "Jane",
                LastName = "Smith"
            };

            // Act & Assert
            var result = validator.TestValidate(author);
            result.ShouldNotHaveValidationErrorFor(a => a.LastName);
        }

        #endregion

        #region AuthorValidator - ShouldHaveValidationErrorFor

        /// <summary>
        /// Test 15: AuthorValidator - Null first name should have validation error for FirstName.
        /// </summary>
        [TestMethod]
        public void AuthorValidator_NullFirstName_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new AuthorValidator();
            var author = new Author
            {
                FirstName = null,
                LastName = "Doe"
            };

            // Act & Assert
            var result = validator.TestValidate(author);
            result.ShouldHaveValidationErrorFor(a => a.FirstName);
        }

        /// <summary>
        /// Test 16: AuthorValidator - Empty first name should have validation error for FirstName.
        /// </summary>
        [TestMethod]
        public void AuthorValidator_EmptyFirstName_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new AuthorValidator();
            var author = new Author
            {
                FirstName = string.Empty,
                LastName = "Doe"
            };

            // Act & Assert
            var result = validator.TestValidate(author);
            result.ShouldHaveValidationErrorFor(a => a.FirstName);
        }

        /// <summary>
        /// Test 17: AuthorValidator - Null last name should have validation error for LastName.
        /// </summary>
        [TestMethod]
        public void AuthorValidator_NullLastName_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new AuthorValidator();
            var author = new Author
            {
                FirstName = "John",
                LastName = null
            };

            // Act & Assert
            var result = validator.TestValidate(author);
            result.ShouldHaveValidationErrorFor(a => a.LastName);
        }

        /// <summary>
        /// Test 18: AuthorValidator - Empty last name should have validation error for LastName.
        /// </summary>
        [TestMethod]
        public void AuthorValidator_EmptyLastName_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new AuthorValidator();
            var author = new Author
            {
                FirstName = "Jane",
                LastName = string.Empty
            };

            // Act & Assert
            var result = validator.TestValidate(author);
            result.ShouldHaveValidationErrorFor(a => a.LastName);
        }

        /// <summary>
        /// Test 19: AuthorValidator - Whitespace first name should have validation error for FirstName.
        /// </summary>
        [TestMethod]
        public void AuthorValidator_WhitespaceFirstName_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new AuthorValidator();
            var author = new Author
            {
                FirstName = "   ",
                LastName = "Doe"
            };

            // Act & Assert
            var result = validator.TestValidate(author);
            result.ShouldHaveValidationErrorFor(a => a.FirstName);
        }

        #endregion

        #region ReaderValidator - ShouldNotHaveValidationErrorFor

        /// <summary>
        /// Test 20: ReaderValidator - Valid reader should not have validation errors for FirstName.
        /// </summary>
        [TestMethod]
        public void ReaderValidator_ValidFirstName_ShouldNotHaveValidationErrorFor()
        {
            // Arrange
            var validator = new ReaderValidator();
            var reader = new Reader
            {
                FirstName = "Jane",
                LastName = "Smith",
                Address = "123 Main Street",
                Email = "jane@example.com",
                PhoneNumber = "555-1234"
            };

            // Act & Assert
            var result = validator.TestValidate(reader);
            result.ShouldNotHaveValidationErrorFor(r => r.FirstName);
        }

        /// <summary>
        /// Test 21: ReaderValidator - Valid reader should not have validation errors for Email.
        /// </summary>
        [TestMethod]
        public void ReaderValidator_ValidEmail_ShouldNotHaveValidationErrorFor()
        {
            // Arrange
            var validator = new ReaderValidator();
            var reader = new Reader
            {
                FirstName = "Jane",
                LastName = "Smith",
                Address = "123 Main Street",
                Email = "jane.smith@example.com",
                PhoneNumber = "555-1234"
            };

            // Act & Assert
            var result = validator.TestValidate(reader);
            result.ShouldNotHaveValidationErrorFor(r => r.Email);
        }

        /// <summary>
        /// Test 22: ReaderValidator - Valid reader should not have validation errors for Address.
        /// </summary>
        [TestMethod]
        public void ReaderValidator_ValidAddress_ShouldNotHaveValidationErrorFor()
        {
            // Arrange
            var validator = new ReaderValidator();
            var reader = new Reader
            {
                FirstName = "Jane",
                LastName = "Smith",
                Address = "456 Oak Avenue, City, Country",
                Email = "jane@example.com",
                PhoneNumber = "555-1234"
            };

            // Act & Assert
            var result = validator.TestValidate(reader);
            result.ShouldNotHaveValidationErrorFor(r => r.Address);
        }

        #endregion

        #region ReaderValidator - ShouldHaveValidationErrorFor

        /// <summary>
        /// Test 23: ReaderValidator - Null first name should have validation error for FirstName.
        /// </summary>
        [TestMethod]
        public void ReaderValidator_NullFirstName_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new ReaderValidator();
            var reader = new Reader
            {
                FirstName = null,
                LastName = "Smith",
                Address = "123 Main Street",
                Email = "jane@example.com",
                PhoneNumber = "555-1234"
            };

            // Act & Assert
            var result = validator.TestValidate(reader);
            result.ShouldHaveValidationErrorFor(r => r.FirstName);
        }

        /// <summary>
        /// Test 24: ReaderValidator - Invalid email should have validation error for Email.
        /// </summary>
        [TestMethod]
        public void ReaderValidator_InvalidEmail_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new ReaderValidator();
            var reader = new Reader
            {
                FirstName = "Jane",
                LastName = "Smith",
                Address = "123 Main Street",
                Email = "invalid-email-without-at",
                PhoneNumber = "555-1234"
            };

            // Act & Assert
            var result = validator.TestValidate(reader);
            result.ShouldHaveValidationErrorFor(r => r.Email);
        }

        /// <summary>
        /// Test 25: ReaderValidator - Null address should have validation error for Address.
        /// </summary>
        [TestMethod]
        public void ReaderValidator_NullAddress_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new ReaderValidator();
            var reader = new Reader
            {
                FirstName = "Jane",
                LastName = "Smith",
                Address = null,
                Email = "jane@example.com",
                PhoneNumber = "555-1234"
            };

            // Act & Assert
            var result = validator.TestValidate(reader);
            result.ShouldHaveValidationErrorFor(r => r.Address);
        }

        /// <summary>
        /// Test 26: ReaderValidator - Empty last name should have validation error for LastName.
        /// </summary>
        [TestMethod]
        public void ReaderValidator_EmptyLastName_ShouldHaveValidationErrorFor()
        {
            // Arrange
            var validator = new ReaderValidator();
            var reader = new Reader
            {
                FirstName = "Jane",
                LastName = string.Empty,
                Address = "123 Main Street",
                Email = "jane@example.com",
                PhoneNumber = "555-1234"
            };

            // Act & Assert
            var result = validator.TestValidate(reader);
            result.ShouldHaveValidationErrorFor(r => r.LastName);
        }

        #endregion

        #region EditionValidator - ShouldNotHaveValidationErrorFor

        /// <summary>
        /// Test 27: EditionValidator - Valid edition should not have validation errors for Publisher.
        /// </summary>
        [TestMethod]
        public void EditionValidator_ValidPublisher_ShouldNotHaveValidationErrorFor()
        {
            // Arrange
            var validator = new EditionValidator();
            var edition = new Edition
            {
                BookId = 1,
                Publisher = "Publishing House Inc.",
                Year = 2023,
                EditionNumber = 1,
                PageCount = 300,
                BookType = "Hardcover"
            };

            // Act & Assert
            var result = validator.TestValidate(edition);
            result.ShouldNotHaveValidationErrorFor(e => e.Publisher);
        }

       

        
        #endregion
    }
}