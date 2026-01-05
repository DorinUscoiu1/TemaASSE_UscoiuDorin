// <copyright file="IReaderService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System.Collections.Generic;
    using Domain.Models;

    /// <summary>
    /// Service interface for reader operations with validation.
    /// </summary>
    public interface IReaderService
    {
        /// <summary>
        /// Gets all readers.
        /// </summary>
        IEnumerable<Reader> GetAllReaders();

        /// <summary>
        /// Gets a reader by ID.
        /// </summary>
        Reader GetReaderById(int readerId);

        /// <summary>
        /// Gets staff members.
        /// </summary>
        IEnumerable<Reader> GetStaffMembers();

        /// <summary>
        /// Gets regular readers.
        /// </summary>
        IEnumerable<Reader> GetRegularReaders();

        /// <summary>
        /// Creates a new reader with validation.
        /// Validates: name consistency, at least one contact method.
        /// </summary>
        void CreateReader(Reader reader);

        /// <summary>
        /// Updates a reader.
        /// </summary>
        void UpdateReader(Reader reader);

        /// <summary>
        /// Deletes a reader.
        /// </summary>
        void DeleteReader(int readerId);

        /// <summary>
        /// Validates reader data consistency.
        /// </summary>
        bool ValidateReader(Reader reader);
    }
}