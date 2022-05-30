namespace GraphQL.Extensions.Authentication.Providers
{
    public interface IAuthenticationProvider
    {
        void Authenticate(string token);
    }
}
