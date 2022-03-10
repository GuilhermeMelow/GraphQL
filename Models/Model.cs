namespace GraphQL.Models
{
    public abstract class Model
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}
