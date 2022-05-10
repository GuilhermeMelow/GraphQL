using GraphQL.Mutations;
using GraphQL.Services.Repositories;

namespace GraphQL.Operations.Queries
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