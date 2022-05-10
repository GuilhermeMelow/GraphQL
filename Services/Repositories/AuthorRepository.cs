using GraphQL.Mutations;

namespace GraphQL.Services.Repositories
{
    public class AuthorRepository : BaseRepository<Author>
    {
        public AuthorRepository() : base(new()
        {
            new Author { Id = Guid.Parse("A7CC841A-A82A-43C5-BFC4-2293A83C1836"), Name = "TesteAutor1" },
            new Author { Id = Guid.Parse("F7E7D48D-DCFA-499A-83F6-1295BB5F3009"), Name = "TesteAutor2" }
        })
        { }
    }
}
