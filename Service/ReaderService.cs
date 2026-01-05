// <copyright file="ReaderService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using Data.Repositories;
    using Domain.Models;

    /// <summary>
    /// Service implementation for reader operations with validation.
    /// Enforces: name consistency, contact information validation.
    /// </summary>
    public class ReaderService : IReaderService
    {
        private readonly IReader readerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderService"/> class.
        /// </summary>
        /// <param name="readerRepository">The reader repository.</param>
        public ReaderService(IReader readerRepository)
        {
            this.readerRepository = readerRepository;
        }

        /// <summary>
        /// Gets all readers.
        /// </summary>
        public IEnumerable<Reader> GetAllReaders()
        {
            return this.readerRepository.GetAll();
        }

        /// <summary>
        /// Gets a reader by ID.
        /// </summary>
        public Reader GetReaderById(int readerId)
        {
            return this.readerRepository.GetById(readerId);
        }

        /// <summary>
        /// Gets staff members.
        /// </summary>
        public IEnumerable<Reader> GetStaffMembers()
        {
            return this.readerRepository.GetStaffMembers();
        }

        /// <summary>
        /// Gets regular readers.
        /// </summary>
        public IEnumerable<Reader> GetRegularReaders()
        {
            return this.readerRepository.GetRegularReaders();
        }

        /// <summary>
        /// Creates a new reader with comprehensive validation.
        /// Rule 1: Names must be consistent and non-empty
        /// Rule 2: At least one contact method (phone or email)
        /// Rule 3: Address is required.
        /// </summary>
        public void CreateReader(Reader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!this.ValidateReader(reader))
            {
                throw new InvalidOperationException("Reader validation failed.");
            }

            reader.RegistrationDate = DateTime.Now;
            this.readerRepository.Add(reader);
        }

        /// <summary>
        /// Updates a reader.
        /// </summary>
        public void UpdateReader(Reader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!this.ValidateReader(reader))
            {
                throw new InvalidOperationException("Reader validation failed.");
            }

            this.readerRepository.Update(reader);
        }

        /// <summary>
        /// Deletes a reader.
        /// </summary>
        public void DeleteReader(int readerId)
        {
            this.readerRepository.Delete(readerId);
        }

        /// <summary>
        /// Validates reader data consistency.
        /// </summary>
        public bool ValidateReader(Reader reader)
        {
            if (string.IsNullOrWhiteSpace(reader.FirstName) || string.IsNullOrWhiteSpace(reader.LastName))
            {
                return false;
            }

            var hasPhone = !string.IsNullOrWhiteSpace(reader.PhoneNumber);
            var hasEmail = !string.IsNullOrWhiteSpace(reader.Email);
            if (!hasPhone && !hasEmail)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(reader.Address))
            {
                return false;
            }

            if (hasEmail && !this.IsValidEmail(reader.Email))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Simple email validation.
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}