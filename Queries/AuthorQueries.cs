using GraphQL.Models;
using GraphQL.Repositories;

namespace GraphQL.Queries
{
    [ExtendObjectType(typeof(RootQuery))]
    public class AuthorQueries
    {
        private readonly AuthorRepository authorRepository;

        public AuthorQueries(AuthorRepository authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        public Author? GetAuthor(Guid id)
        {
            return authorRepository[id];
        }

        public IEnumerable<Author> GetAuthors() => authorRepository.Items;
    }
}