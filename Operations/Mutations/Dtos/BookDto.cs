namespace GraphQL.Operations.Mutations.Dtos
{
    public record BookDto(Guid Id, string Title, Guid AuthorId);
}
