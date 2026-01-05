using System.Collections.Generic;
using Domain.Models;

namespace Data.Repositories
{
    /// <summary>
    /// Repository interface for Book-specific operations.
    /// </summary>
    public interface IBook
    {
        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        IEnumerable<Domain.Models.Book> GetBooksByAuthor(int authorId);

        /// <summary>
        /// Gets books by domain ID.
        /// </summary>
        IEnumerable<Domain.Models.Book> GetBooksByDomain(int domainId);

        /// <summary>
        /// Gets books with available copies.
        /// </summary>
        IEnumerable<Domain.Models.Book> GetAvailableBooks();

        /// <summary>
        /// Gets books by ISBN.
        /// </summary>
        Domain.Models.Book GetByISBN(string isbn);
    }
}