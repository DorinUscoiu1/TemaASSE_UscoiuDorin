// <copyright file="ReaderService.cs" company="Transilvania University of Brasov">
// Copyright © 2026 Uscoiu Dorin. All rights reserved.
// </copyright>

namespace Service
{
    using System;
    using System.Collections.Generic;
    using Data.Repositories;
    using Data.Validators;
    using Domain.Models;
    using FluentValidation;

    /// <summary>
    /// Service implementation for reader operations with validation.
    /// Enforces: name consistency, contact information validation.
    /// </summary>
    public class ReaderService : IReaderService
    {
        private readonly IReader readerRepository;
        private readonly IValidator<Reader> readerValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderService"/> class.
        /// </summary>
        /// <param name="readerRepository">The reader repository.</param>
        public ReaderService(IReader readerRepository)
        {
            this.readerRepository = readerRepository ?? throw new ArgumentNullException(nameof(readerRepository));
            this.readerValidator = new ReaderValidator();
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

            // Validate using FluentValidation
            var validationResult = this.readerValidator.Validate(reader);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors);
                throw new ValidationException(errors);
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

            // Validate using FluentValidation
            var validationResult = this.readerValidator.Validate(reader);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors);
                throw new ValidationException(errors);
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
            if (reader == null)
            {
                return false;
            }

            var validationResult = this.readerValidator.Validate(reader);
            return validationResult.IsValid;
        }

        /// <summary>
        /// Gets staff readers.
        /// </summary>
        public IEnumerable<Reader> GetStaffReaders()
        {
            return this.readerRepository.GetStaffMembers();
        }
    }
}