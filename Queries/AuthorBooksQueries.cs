using GraphQL.Models;
using GraphQL.Repositories;

namespace GraphQL.Queries
{
    [ExtendObjectType(typeof(RootQuery))]
    public class AuthorBooksQueries
    {
        private readonly BookRepository bookRepository;

        public AuthorBooksQueries(BookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public IEnumerable<Book> GetBooksByAuthor(Guid authorId)
        {
            return bookRepository.Items.Where(b => b.AuthorId == authorId);
        }
    }
}
