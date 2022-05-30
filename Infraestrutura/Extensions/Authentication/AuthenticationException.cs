namespace GraphQL.Extensions.Authentication
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message = "Unauthorized") : base(message) { }
    }
}
