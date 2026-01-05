// <copyright file="ReaderDataService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Models;

    /// <summary>
    /// Repository implementation for Reader-specific operations.
    /// </summary>
    public class ReaderDataService : IReader
    {
        private readonly LibraryDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderDataService"/> class.
        /// </summary>
        /// <param name="context">The library database context.</param>
        public ReaderDataService(LibraryDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets all readers.
        /// </summary>
        public IEnumerable<Reader> GetAll()
        {
            return this.context.Readers.ToList();
        }

        /// <summary>
        /// Gets a reader by ID.
        /// </summary>
        public Reader GetById(int id)
        {
            return this.context.Readers.Find(id);
        }

        /// <summary>
        /// Gets readers by staff status.
        /// </summary>
        public IEnumerable<Reader> GetStaffMembers()
        {
            return this.context.Readers
                .Where(r => r.IsStaff)
                .ToList();
        }

        /// <summary>
        /// Gets all non-staff readers.
        /// </summary>
        public IEnumerable<Reader> GetRegularReaders()
        {
            return this.context.Readers
                .Where(r => !r.IsStaff)
                .ToList();
        }

        /// <summary>
        /// Finds a reader by email.
        /// </summary>
        public Reader GetByEmail(string email)
        {
            return this.context.Readers
                .FirstOrDefault(r => r.Email == email);
        }

        /// <summary>
        /// Gets readers with active borrowings.
        /// </summary>
        public IEnumerable<Reader> GetReadersWithActiveBorrowings()
        {
            return this.context.Readers
                .Where(r => r.BorrowingRecords.Any(b => b.IsActive))
                .ToList();
        }

        /// <summary>
        /// Adds a new reader.
        /// </summary>
        public void Add(Reader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.context.Readers.Add(reader);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Updates a reader.
        /// </summary>
        public void Update(Reader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var existingReader = this.context.Readers.Find(reader.Id);
            if (existingReader != null)
            {
                existingReader.FirstName = reader.FirstName;
                existingReader.LastName = reader.LastName;
                existingReader.Address = reader.Address;
                existingReader.PhoneNumber = reader.PhoneNumber;
                existingReader.Email = reader.Email;
                existingReader.IsStaff = reader.IsStaff;
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a reader.
        /// </summary>
        public void Delete(int id)
        {
            var reader = this.context.Readers.Find(id);
            if (reader != null)
            {
                this.context.Readers.Remove(reader);
                this.context.SaveChanges();
            }
        }
    }
}