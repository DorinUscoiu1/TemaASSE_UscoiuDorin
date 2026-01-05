// <copyright file="IReader.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Data.Repositories
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Repository interface for Reader-specific operations.
    /// </summary>
    public interface IReader
    {
        /// <summary>
        /// Gets all readers.
        /// </summary>
        IEnumerable<Reader> GetAll();

        /// <summary>
        /// Gets a reader by ID.
        /// </summary>
        Reader GetById(int id);

        /// <summary>
        /// Gets readers by staff status.
        /// </summary>
        IEnumerable<Reader> GetStaffMembers();

        /// <summary>
        /// Gets all non-staff readers.
        /// </summary>
        IEnumerable<Reader> GetRegularReaders();

        /// <summary>
        /// Finds a reader by email.
        /// </summary>
        Reader GetByEmail(string email);

        /// <summary>
        /// Gets readers with active borrowings.
        /// </summary>
        IEnumerable<Reader> GetReadersWithActiveBorrowings();

        /// <summary>
        /// Adds a new reader.
        /// </summary>
        void Add(Reader reader);

        /// <summary>
        /// Updates a reader.
        /// </summary>
        void Update(Reader reader);

        /// <summary>
        /// Deletes a reader.
        /// </summary>
        void Delete(int id);
    }
}