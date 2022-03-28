using GraphQL.Models;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GraphQL.Subscriptions
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
        [Topic("RemovedBook")]
        public Book RemovedBook([EventMessage] Book book) => book;

        [Subscribe]
        [Topic("ChangedBook")]
        public Book ChangedBook([EventMessage] Book book) => book;
    }
}
