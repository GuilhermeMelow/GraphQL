using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace GraphQL.Configuration.ApiKey
{
    public static class ApiKeyAuthNDefaults
    {
        public const string AuthenticationScheme = "ApiKey";
    }

    public class ApiKeyAuthN : AuthenticationHandler<ApiKeyAuthNOptions>
    {
        public ApiKeyAuthN(
            IOptionsMonitor<ApiKeyAuthNOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKey = ParseApiKey();

            if (string.IsNullOrEmpty(apiKey)) return Task.FromResult(AuthenticateResult.NoResult());

            if (string.Compare(apiKey, Options.ApiKey, StringComparison.Ordinal) != 0)
            {
                return Task.FromResult(AuthenticateResult.Fail($"Invalid Api Key provided."));
            }

            var principal = BuildPrincipal(Scheme.Name, Options.ApiKey, Options.ClaimsIssuer ?? "ApiKey");

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, apiKey)));
        }

        protected string? ParseApiKey()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var apiKeys)) return string.Empty;

            return apiKeys.FirstOrDefault();
        }

        private static ClaimsPrincipal BuildPrincipal(string schemeName,
            string name,
            string issuer,
            IEnumerable<Claim>? claims = default(List<Claim>))
        {
            var identity = new ClaimsIdentity(schemeName);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, name, ClaimValueTypes.String, issuer));
            identity.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, issuer));

            identity.AddClaims(claims ?? Enumerable.Empty<Claim>());

            return new ClaimsPrincipal(identity);
        }
    }
}
