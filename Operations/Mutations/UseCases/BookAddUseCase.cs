using AutoMapper;
using GraphQL.GraphExceptions.Book;
using GraphQL.Mutations;
using GraphQL.Operations.Mutations.Dtos;
using GraphQL.Services.Repositories;
using HotChocolate.Subscriptions;

namespace GraphQL.Operations.Mutations.UseCases
{
    public interface IBookAddUseCase
    {
        Task<Book> Handle(BookDto bookInput);
    }

    public class BookAddUseCase : IBookAddUseCase
    {
        private readonly BookRepository repository;
        private readonly IMapper mapper;
        private readonly ITopicEventSender sender;

        public BookAddUseCase(BookRepository repository, IMapper mapper, ITopicEventSender sender)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.sender = sender;
        }

        public async Task<Book> Handle(BookDto bookInput)
        {
            if (IsRegistered(bookInput.Id)) throw new BookAlredyRegisteredException("O dado já foi cadastro");

            var book = mapper.Map<Book>(bookInput);

            await repository.Add(book);

            await sender.SendAsync($"{book.AuthorId}_PublishedBook", book);

            return book;
        }

        private bool IsRegistered(Guid? id) => repository.Items.Any(b => b.Id == id);
    }
}
