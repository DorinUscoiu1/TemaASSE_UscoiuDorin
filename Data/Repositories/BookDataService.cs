using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Data.Repositories
{
    /// <summary>
    /// Repository implementation for Book-specific operations.
    /// </summary>
    public class BookDataService : IBook
    {
        private readonly LibraryDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookDataService"/> class.
        /// </summary>
        /// <param name="context">The library database context.</param>
        public BookDataService(LibraryDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets books by author ID.
        /// </summary>
        /// <param name="authorId">The author identifier.</param>
        /// <returns>A collection of books by the specified author.</returns>
        public IEnumerable<Domain.Models.Book> GetBooksByAuthor(int authorId)
        {
            return this.context.Books
                .Where(b => b.Authors.Any(a => a.Id == authorId))
                .ToList();
        }

        /// <summary>
        /// Gets books by domain ID.
        /// </summary>
        /// <param name="domainId">The domain identifier.</param>
        /// <returns>A collection of books in the specified domain.</returns>
        public IEnumerable<Domain.Models.Book> GetBooksByDomain(int domainId)
        {
            return this.context.Books
                .Where(b => b.Domains.Any(d => d.Id == domainId))
                .ToList();
        }

        /// <summary>
        /// Gets books with available copies.
        /// </summary>
        /// <returns>A collection of books that have available copies.</returns>
        public IEnumerable<Domain.Models.Book> GetAvailableBooks()
        {
            return this.context.Books
                .Where(b => b.GetAvailableCopies() > 0)
                .ToList();
        }

        /// <summary>
        /// Gets books by ISBN.
        /// </summary>
        /// <param name="isbn">The ISBN value.</param>
        /// <returns>The book with the specified ISBN, or null if not found.</returns>
        public Domain.Models.Book GetByISBN(string isbn)
        {
            return this.context.Books
                .FirstOrDefault(b => b.ISBN == isbn);
        }
    }
}