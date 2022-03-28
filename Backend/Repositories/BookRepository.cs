using GraphQL.Models;

namespace GraphQL.Repositories
{
    public class BookRepository : BaseRepository<Book>
    {
        public BookRepository() : base(new()
        {
            new Book { Title = "Teste0", AuthorId = Guid.Parse("A7CC841A-A82A-43C5-BFC4-2293A83C1836") },
            new Book { Title = "Teste1", AuthorId = Guid.Parse("F7E7D48D-DCFA-499A-83F6-1295BB5F3009") },
            new Book { Title = "Teste2", AuthorId = Guid.Parse("F7E7D48D-DCFA-499A-83F6-1295BB5F3009") },
            new Book { Title = "Teste3", AuthorId = Guid.Parse("A7CC841A-A82A-43C5-BFC4-2293A83C1836") },
        })
        { }
    }
}