using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GraphQL.Extensions
{
    public class CustomClaimsTransformer : IClaimsTransformation
    {
        private readonly UserManager<ApplicationUser> userManager;

        public CustomClaimsTransformer(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (TryGetUser(principal, out var user) || user == null) return Task.FromResult(principal);

            principal.Identities.First().AddClaims(new Claim[]
            {
                new Claim("Id", user.Id),
                new Claim("ApiKey", user.ApiKey ?? string.Empty),
            });

            return Task.FromResult(principal);
        }

        private bool TryGetUser(ClaimsPrincipal principal, out ApplicationUser? user)
        {
            user = null;

            if (principal.Identity == null || !principal.Identity.IsAuthenticated)
                return false;

            if (principal.Claims.Any() || !principal.HasClaim(c => c.Type == ClaimTypes.Email) || principal.HasClaim(c => c.Type == "Id"))
                return false;

            user = userManager.Users.FirstOrDefault(c => c.Email == principal.FindFirstValue(ClaimTypes.Email));

            return true;
        }
    }
}
