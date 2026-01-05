// <copyright file="EditionService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using Data.Repositories;
    using Domain.Models;

    /// <summary>
    /// Service implementation for edition operations.
    /// Manages book editions with validation.
    /// </summary>
    public class EditionService : IEditionService
    {
        private readonly IEdition editionRepository;
        private readonly IBook bookRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditionService"/> class.
        /// </summary>
        /// <param name="editionRepository">The edition repository.</param>
        /// <param name="bookRepository">The book repository.</param>
        public EditionService(IEdition editionRepository, IBook bookRepository)
        {
            this.editionRepository = editionRepository;
            this.bookRepository = bookRepository;
        }

        /// <summary>
        /// Gets all editions.
        /// </summary>
        public IEnumerable<Edition> GetAllEditions()
        {
            return this.editionRepository.GetAll();
        }

        /// <summary>
        /// Gets an edition by ID.
        /// </summary>
        public Edition GetEditionById(int editionId)
        {
            return this.editionRepository.GetById(editionId);
        }

        /// <summary>
        /// Gets editions by book ID.
        /// </summary>
        public IEnumerable<Edition> GetEditionsByBook(int bookId)
        {
            return this.editionRepository.GetByBookId(bookId);
        }

        /// <summary>
        /// Gets editions by publisher.
        /// </summary>
        public IEnumerable<Edition> GetEditionsByPublisher(string publisher)
        {
            return this.editionRepository.GetByPublisher(publisher);
        }

        /// <summary>
        /// Creates a new edition with validation.
        /// </summary>
        public void CreateEdition(Edition edition)
        {
            if (edition == null)
            {
                throw new ArgumentNullException(nameof(edition));
            }

            if (string.IsNullOrWhiteSpace(edition.Publisher))
            {
                throw new ArgumentException("Publisher is required.");
            }

            if (edition.Year <= 0)
            {
                throw new ArgumentException("Year must be a valid year.");
            }

            if (edition.PageCount <= 0)
            {
                throw new ArgumentException("Page count must be greater than zero.");
            }

            if (edition.EditionNumber <= 0)
            {
                throw new ArgumentException("Edition number must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(edition.BookType))
            {
                throw new ArgumentException("Book type is required.");
            }

            // Verify book exists
            var book = this.bookRepository.GetById(edition.BookId);
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            this.editionRepository.Add(edition);
        }

        /// <summary>
        /// Updates an edition.
        /// </summary>
        public void UpdateEdition(Edition edition)
        {
            if (edition == null)
            {
                throw new ArgumentNullException(nameof(edition));
            }

            if (string.IsNullOrWhiteSpace(edition.Publisher))
            {
                throw new ArgumentException("Publisher is required.");
            }

            if (edition.Year <= 0 || edition.PageCount <= 0)
            {
                throw new ArgumentException("Year and page count must be valid.");
            }

            this.editionRepository.Update(edition);
        }

        /// <summary>
        /// Deletes an edition.
        /// </summary>
        public void DeleteEdition(int editionId)
        {
            var edition = this.editionRepository.GetById(editionId);
            if (edition != null)
            {
                this.editionRepository.Delete(editionId);
            }
        }
    }
}