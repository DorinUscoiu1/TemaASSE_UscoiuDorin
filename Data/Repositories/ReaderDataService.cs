using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Data.Repositories
{
    /// <summary>
    /// Repository implementation for Reader-specific operations.
    /// </summary>
    public class ReaderDataService :IReader
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
        /// Gets readers by staff status.
        /// </summary>
        /// <returns>A collection of staff members.</returns>
        public IEnumerable<Domain.Models.Reader> GetStaffMembers()
        {
            return this.context.Readers
                .Where(r => r.IsStaff)
                .ToList();
        }

        /// <summary>
        /// Gets all non-staff readers.
        /// </summary>
        /// <returns>A collection of regular readers.</returns>
        public IEnumerable<Domain.Models.Reader> GetRegularReaders()
        {
            return this.context.Readers
                .Where(r => !r.IsStaff)
                .ToList();
        }

        /// <summary>
        /// Finds a reader by email.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>The reader with the specified email, or null if not found.</returns>
        public Domain.Models.Reader GetByEmail(string email)
        {
            return this.context.Readers
                .FirstOrDefault(r => r.Email == email);
        }

        /// <summary>
        /// Gets readers with active borrowings.
        /// </summary>
        /// <returns>A collection of readers with active borrowing records.</returns>
        public IEnumerable<Domain.Models.Reader> GetReadersWithActiveBorrowings()
        {
            return this.context.Readers
                .Where(r => r.BorrowingRecords.Any(b => b.IsActive))
                .ToList();
        }
    }
}