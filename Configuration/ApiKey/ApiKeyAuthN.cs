using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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

    public class ApiKeyAuthN : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ApiKeyAuthN(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserManager<ApplicationUser> userManager) : base(options, logger, encoder, clock)
        {
            this.userManager = userManager;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKey = ParseApiKey();

            if (string.IsNullOrEmpty(apiKey)) return Task.FromResult(AuthenticateResult.NoResult());

            var user = userManager.Users.FirstOrDefault(c => c.ApiKey == apiKey);

            if (user == null) return Task.FromResult(AuthenticateResult.Fail($"Invalid Api Key provided."));

            var principal = BuildPrincipal(Scheme.Name, user, Options.ClaimsIssuer ?? "ApiKey");

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, user.Email)));
        }

        protected string? ParseApiKey()
        {
            return Request.Headers.TryGetValue("Authorization", out var apiKeys) ? apiKeys.FirstOrDefault() : string.Empty;
        }

        private static ClaimsPrincipal BuildPrincipal(string schemeName,
            ApplicationUser user,
            string issuer,
            IEnumerable<Claim>? claims = default(List<Claim>))
        {
            var identity = new ClaimsIdentity(schemeName);

            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String, issuer));

            identity.AddClaims(claims ?? Enumerable.Empty<Claim>());

            return new ClaimsPrincipal(identity);
        }
    }
}
