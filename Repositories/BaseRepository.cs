using GraphQL.Models;

namespace GraphQL.Repositories
{
    public abstract class BaseRepository<T> where T : Model
    {
        protected readonly List<T> items;

        public BaseRepository(List<T> items)
        {
            this.items = items;
        }

        public IEnumerable<T> Items => items;

        public T this[Guid id] => items.Single(i => i.Id == id);

        public Task Add(T item)
        {
            items.Add(item);

            return Task.CompletedTask;
        }

        public Task Remover(T item)
        {
            items.Remove(item);

            return Task.CompletedTask;
        }

        public Task Alterar(T itemAtualizado)
        {
            var itemIndex = items.FindIndex(i => i.Id == itemAtualizado.Id);

            items[itemIndex] = itemAtualizado;

            return Task.CompletedTask;
        }
    }
}
