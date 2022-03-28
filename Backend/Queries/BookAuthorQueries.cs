using GraphQL.Models;
using GraphQL.Repositories;

namespace GraphQL.Queries
{
    [ExtendObjectType(typeof(Book),
        IgnoreProperties = new[] { nameof(Book.AuthorId) })]
    internal class BookAuthorQueries
    {
        private readonly AuthorRepository authorRepository;

        public BookAuthorQueries(AuthorRepository authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        [GraphQLName("author")]
        public Author? GetAuthorByBook([Parent] Book book)
        {
            return authorRepository[book.AuthorId];
        }
    }
}
