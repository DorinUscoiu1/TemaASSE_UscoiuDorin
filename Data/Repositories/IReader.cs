using System.Collections.Generic;
using Domain.Models;

namespace Data.Repositories
{
    /// <summary>
    /// Repository interface for Reader-specific operations.
    /// </summary>
    public interface IReader
    {
        /// <summary>
        /// Gets readers by staff status.
        /// </summary>
        IEnumerable<Domain.Models.Reader> GetStaffMembers();

        /// <summary>
        /// Gets all non-staff readers.
        /// </summary>
        IEnumerable<Domain.Models.Reader> GetRegularReaders();

        /// <summary>
        /// Finds a reader by email.
        /// </summary>
        Domain.Models.Reader GetByEmail(string email);

        /// <summary>
        /// Gets readers with active borrowings.
        /// </summary>
        IEnumerable<Domain.Models.Reader> GetReadersWithActiveBorrowings();
    }
}