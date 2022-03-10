namespace GraphQL.GraphExceptions
{
    [GraphQLName("RootError")]
    public interface IRootError
    {
        string Message { get; }
        int Code { get; }
    }
}