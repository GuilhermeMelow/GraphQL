namespace GraphQL.Mutations
{
    public class UserDto
    {
        public string Username { get; init; }
        public string Password { internal get; init; }
    }

    public class UserResult
    {
        public string Username { get; init; }
        public string Token { get; init; }
    }
}
