using GraphQL.Configuration.ApiKey;
using Microsoft.AspNetCore.Authentication;

namespace GraphQL.Extensions
{
    public static class ApiKeyAuthExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyAuthNOptions>? configureOptions)
            => AddApiKey(builder, ApiKeyAuthNDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyAuthNOptions>? configureOptions)
            => builder.AddScheme<ApiKeyAuthNOptions, ApiKeyAuthN>(authenticationScheme, configureOptions);
    }
}
