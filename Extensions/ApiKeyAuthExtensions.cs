using GraphQL.Configuration.ApiKey;
using Microsoft.AspNetCore.Authentication;

namespace GraphQL.Extensions
{
    public static class ApiKeyAuthExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions>? configureOptions)
            => AddApiKey(builder, ApiKeyAuthNDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, Action<AuthenticationSchemeOptions>? configureOptions)
            => builder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthN>(authenticationScheme, configureOptions);
    }
}
