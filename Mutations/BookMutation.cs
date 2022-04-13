using AutoMapper;
using GraphQL.GraphExceptions.Book;
using GraphQL.Models;
using GraphQL.Repositories;
using GraphQL.UseCases;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Subscriptions;

namespace GraphQL.Mutations
{

    [ExtendObjectType(typeof(Mutation))]
    public class BookMutation
    {
        private readonly BookRepository repository;
        private readonly IMapper mapper;
        private readonly ITopicEventSender sender;

        public BookMutation(BookRepository repository, IMapper mapper, ITopicEventSender sender)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.sender = sender;
        }

        [Authorize]
        public Task<Book> AddBook(BookDto bookInput, [Service] IBookAddUseCase useCase) => useCase.Handle(bookInput);

        [Authorize]
        public async Task<Book> UpdateBook(Guid id, BookDto bookInput)
        {
            if (!IsRegistered(bookInput.Id)) throw new BookNotFoundException("Não foi encontrado.");

            if (id != bookInput.Id) throw new DivergentGuidException();

            var book = mapper.Map<Book>(bookInput);

            await repository.Alterar(book);

            await sender.SendAsync("ChangedBook", book);

            return book;
        }

        [Authorize]
        public async Task<Book> RemoveBook(Guid id)
        {
            if (!IsRegistered(id)) throw new BookNotFoundException();

            var book = repository[id];

            await repository.Remover(book);

            await sender.SendAsync("RemovedBook", book);

            return book;
        }

        private bool IsRegistered(Guid? id)
        {
            return repository.Items.Any(b => b.Id == id);
        }
    }
}
