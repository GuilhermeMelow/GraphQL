using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GraphQL.Extensions.UserCache
{
    public class UserCacheMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ApplicationUserCache userCache;

        public UserCacheMiddleware(RequestDelegate next, ApplicationUserCache userCache)
        {
            this.next = next;
            this.userCache = userCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isAuthenticated = context.User.Identity != null && context.User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                var email = context.User.FindFirstValue(ClaimTypes.Email);

                if (!userCache.HasCache && email != null)
                {
                    await userCache.DefineCacheAsync(email);
                }
            }

            await next(context);
        }
    }

    public static class UserCacheMiddlewareExtensions
    {
        public static void UserCacheUser(this WebApplication app)
        {
            app.UseMiddleware<UserCacheMiddleware>();
        }
    }
}
