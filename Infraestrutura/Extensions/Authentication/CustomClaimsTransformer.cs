using GraphQL.Extensions.UserCache;
using GraphQL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GraphQL.Extensions.Authentication
{
    public class CustomClaimsTransformer : IClaimsTransformation
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationUserCache userCache;

        public CustomClaimsTransformer(UserManager<User> userManager, ApplicationUserCache userCache)
        {
            this.userManager = userManager;
            this.userCache = userCache;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!TryGetUser(principal, out var user) || user == null) return Task.FromResult(principal);

            principal.Identities.First().AddClaims(new Claim[]
            {
                new Claim("Id", user.Id),
                new Claim("ApiKey", user.ApiKey ?? string.Empty),
            });

            return Task.FromResult(principal);
        }

        private bool TryGetUser(ClaimsPrincipal principal, out User? user)
        {
            user = userCache.GetCache();

            if (user != null) return true;

            var isNotAuthenticated = principal.Identity == null || !principal.Identity.IsAuthenticated;
            var hasNotEmailClaim = principal.FindFirstValue(ClaimTypes.Email) == null;

            if (isNotAuthenticated && hasNotEmailClaim) return false;

            user = userManager
                    .Users
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Email == principal.FindFirstValue(ClaimTypes.Email));

            return true;
        }
    }
}
