using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Extensions.Authentication.Providers
{
    public class AuthenticationProviderFactory
    {
        private readonly IServiceProvider provider;

        public AuthenticationProviderFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public IAuthenticationProvider GetProvider(string token)
        {
            if (token.Contains("Bearer")) return provider.GetRequiredService<JwtAuthenticatorProvider>();

            return provider.GetRequiredService<ApiAuthenticatorProvider>();
        }
    }
}
