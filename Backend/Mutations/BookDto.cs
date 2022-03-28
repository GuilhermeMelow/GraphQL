namespace GraphQL.Mutations
{
    public class BookDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
    }
}
