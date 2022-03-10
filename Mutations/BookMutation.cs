using AutoMapper;
using GraphQL.GraphExceptions.Book;
using GraphQL.Models;
using GraphQL.Repositories;

namespace GraphQL.Mutations
{
    public class BookMutation
    {
        private readonly BookRepository repository;
        private readonly IMapper mapper;

        public BookMutation(BookRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [Error(typeof(BookAlredyRegisteredException))]
        public async Task<Book> AddBook(BookDto bookInput)
        {
            if (IsRegistered(bookInput.Id)) throw new BookAlredyRegisteredException("O dado já foi cadastro");

            var book = mapper.Map<Book>(bookInput);

            await repository.Add(book);

            return book;
        }

        [Error(typeof(BookErrorFactory))]
        public async Task<Book> UpdateBook([ID] Guid id, BookDto bookInput)
        {
            if (!IsRegistered(bookInput.Id)) throw new BookNotFoundException("Não foi encontrado.");

            if (id != bookInput.Id) throw new DivergentGuidException();

            var book = mapper.Map<Book>(bookInput);

            await repository.Alterar(book);

            return book;
        }

        [Error(typeof(BookErrorFactory))]
        public async Task<Book> RemoveBook([ID] Guid id)
        {
            if (!IsRegistered(id)) throw new BookNotFoundException();

            var book = repository[id];

            await repository.Remover(book);

            return book;
        }

        private bool IsRegistered(Guid? id)
        {
            return repository.Items.Any(b => b.Id == id);
        }
    }
}
