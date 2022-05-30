using GraphQL.Models;
using GraphQL.Services.UserService;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GraphQL.Extensions.Authentication
{
    public class CustomClaimsTransformer : IClaimsTransformation
    {
        private readonly IApplicationUserService userService;

        public CustomClaimsTransformer(IApplicationUserService userService)
        {
            this.userService = userService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);

            if (user == null) return principal;

            principal.Identities.First().AddClaims(new Claim[]
            {
                new Claim("Id", user.Id),
                new Claim("ApiKey", user.ApiKey ?? string.Empty),
            });

            return principal;
        }

        private async Task<User?> GetUserAsync(ClaimsPrincipal principal)
        {
            var user = await userService.GetUserAsync(principal.FindFirstValue(ClaimTypes.Email));

            if (user != null) return null;

            var isNotAuthenticated = principal.Identity == null || !principal.Identity.IsAuthenticated;
            var hasNotEmailClaim = principal.FindFirstValue(ClaimTypes.Email) == null;

            return isNotAuthenticated && hasNotEmailClaim ? null : user;
        }
    }
}
