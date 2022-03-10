using GraphQL.Models;
using GraphQL.Repositories;

namespace GraphQL.Queries
{
    [ExtendObjectType(typeof(RootQuery))]
    public class BookQueries
    {
        private readonly BookRepository repository;

        public BookQueries(BookRepository bookRepository)
        {
            repository = bookRepository;
        }

        public IEnumerable<Book> GetBooks()
        {
            return repository.Items;
        }

        public Book? GetBook(string title)
        {
            return repository.Items.FirstOrDefault(x => x.Title == title);
        }
    }
}