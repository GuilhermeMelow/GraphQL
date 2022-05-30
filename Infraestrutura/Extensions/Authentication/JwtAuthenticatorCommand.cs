using GraphQL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GraphQL.Extensions.Authentication
{
    public interface IAuthenticationProvider
    {
        void Authenticate(string token);
    }

    public class JwtAuthenticatorProvider : IAuthenticationProvider
    {
        private readonly AppSettings settings;

        public JwtAuthenticatorProvider(IOptions<AppSettings> settings)
        {
            this.settings = settings.Value;
        }

        public void Authenticate(string authToken)
        {
            var token = authToken.Replace("Bearer", string.Empty).Trim();

            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(token)) throw new ArgumentException("Irregular format Token.");

            var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true
            }, out _);

            if (claims == null) throw new ArgumentException("Unauthorized (Invalid token)");
        }
    }

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

            if (user == null) throw new ArgumentException("Unauthorized (Invalid token)");
        }
    }

    public class AuthenticationProviderFactory
    {
        private readonly IServiceProvider provider;

        public AuthenticationProviderFactory(ServiceProvider provider)
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
