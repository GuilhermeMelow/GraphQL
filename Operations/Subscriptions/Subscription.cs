using GraphQL.Mutations;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GraphQL.Operations.Subscriptions
{
    public class Subscription
    {
        public ValueTask<ISourceStream<Book>> SubscribeToBooksAsync([Service] ITopicEventReceiver receiver, Guid authorId)
        {
            return receiver.SubscribeAsync<string, Book>($"{authorId}_PublishedBook");
        }

        [Subscribe(With = nameof(SubscribeToBooksAsync))]
        public Book BookPublished([EventMessage] Book book, Guid authorId) => book;

        [Subscribe]
        public Book BookRemoved([EventMessage] Book book) => book;

        [Subscribe]
        public Book BookChanged([EventMessage] Book book) => book;
    }
}
