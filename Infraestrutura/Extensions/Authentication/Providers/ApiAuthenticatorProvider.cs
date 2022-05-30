using GraphQL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Extensions.Authentication.Providers
{
    public class ApiAuthenticatorProvider : IAuthenticationProvider
    {
        private readonly UserManager<User> userManager;

        public ApiAuthenticatorProvider(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public void Authenticate(string token)
        {
            var user = userManager
                .Users
                .AsNoTracking()
                .FirstOrDefault(u => u.ApiKey == token);

            if (user == null) throw new AuthenticationException();
        }
    }
}
