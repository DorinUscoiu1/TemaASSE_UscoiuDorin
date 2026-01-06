// <copyright file="AuthorService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Repositories;
    using Data.Validators;
    using Domain.Models;
    using FluentValidation;

    /// <summary>
    /// Service implementation for author operations with business rule validation.
    /// Enforces: name validation, data consistency.
    /// </summary>
    public class AuthorService : IAuthorService
    {
        private readonly IAuthor authorRepository;
        private readonly IValidator<Author> authorValidator;
        private readonly LibraryConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorService"/> class.
        /// </summary>
        /// <param name="authorRepository">The author repository.</param>
        /// <param name="config">The library configuration.</param>
        public AuthorService(IAuthor authorRepository, LibraryConfiguration config)
        {
            this.authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.authorValidator = new AuthorValidator();
        }

        /// <summary>
        /// Gets all authors.
        /// </summary>
        public IEnumerable<Author> GetAllAuthors()
        {
            return this.authorRepository.GetAll();
        }

        /// <summary>
        /// Gets an author by ID.
        /// </summary>
        public Author GetAuthorById(int authorId)
        {
            return this.authorRepository.GetById(authorId);
        }

        /// <summary>
        /// Gets authors by first name.
        /// </summary>
        public IEnumerable<Author> GetAuthorsByFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return Enumerable.Empty<Author>();
            }

            return this.authorRepository.GetByFirstName(firstName);
        }

        /// <summary>
        /// Gets authors by last name.
        /// </summary>
        public IEnumerable<Author> GetAuthorsByLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return Enumerable.Empty<Author>();
            }

            return this.authorRepository.GetByLastName(lastName);
        }

        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        public IEnumerable<Book> GetBooksByAuthor(int authorId)
        {
            var author = this.authorRepository.GetById(authorId);
            if (author == null)
            {
                return Enumerable.Empty<Book>();
            }

            return author.Books ?? Enumerable.Empty<Book>();
        }

        /// <summary>
        /// Creates a new author with comprehensive validation.
        /// Rule 1: First name is required and non-empty
        /// Rule 2: Last name is required and non-empty
        /// Rule 3: Names must be consistent and not identical
        /// </summary>
        public void CreateAuthor(Author author)
        {
            // Validation 1: Author cannot be null
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // Validation 2: Validate using FluentValidation
            var validationResult = this.authorValidator.Validate(author);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }

            try
            {
                this.authorRepository.Add(author);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create author: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an author.
        /// </summary>
        public void UpdateAuthor(Author author)
        {
            // Validation 1: Author cannot be null
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // Validation 2: Validate using FluentValidation
            var validationResult = this.authorValidator.Validate(author);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }

            try
            {
                this.authorRepository.Update(author);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update author: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes an author.
        /// </summary>
        public void DeleteAuthor(int authorId)
        {
            try
            {
                this.authorRepository.Delete(authorId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete author: {ex.Message}", ex);
            }
        }
    }
}