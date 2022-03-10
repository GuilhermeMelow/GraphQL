namespace GraphQL.Models
{
    public class Book : Model
    {
        public string Title { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
    }
}