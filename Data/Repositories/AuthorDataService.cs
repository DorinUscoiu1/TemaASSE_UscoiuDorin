// <copyright file="AuthorDataService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Models;

    /// <summary>
    /// Repository implementation for Author-specific operations.
    /// </summary>
    public class AuthorDataService : IAuthor
    {
        private readonly LibraryDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorDataService"/> class.
        /// </summary>
        /// <param name="context">The library database context.</param>
        public AuthorDataService(LibraryDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets all authors.
        /// </summary>
        public IEnumerable<Author> GetAll()
        {
            return this.context.Authors.ToList();
        }

        /// <summary>
        /// Gets an author by ID.
        /// </summary>
        public Author GetById(int id)
        {
            return this.context.Authors.Find(id);
        }

        /// <summary>
        /// Gets authors by first name.
        /// </summary>
        public IEnumerable<Author> GetByFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return new List<Author>();
            }

            return this.context.Authors
                .Where(a => a.FirstName.Contains(firstName))
                .ToList();
        }

        /// <summary>
        /// Gets authors by last name.
        /// </summary>
        public IEnumerable<Author> GetByLastName(string lastName)
        {
            return this.context.Authors
                .Where(a => a.LastName == lastName)
                .ToList();
        }

        /// <summary>
        /// Adds a new author.
        /// </summary>
        public void Add(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            this.context.Authors.Add(author);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Updates an author.
        /// </summary>
        public void Update(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            var existingAuthor = this.context.Authors.Find(author.Id);
            if (existingAuthor != null)
            {
                existingAuthor.FirstName = author.FirstName;
                existingAuthor.LastName = author.LastName;
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes an author.
        /// </summary>
        public void Delete(int id)
        {
            var author = this.context.Authors.Find(id);
            if (author != null)
            {
                this.context.Authors.Remove(author);
                this.context.SaveChanges();
            }
        }

        
    }
}