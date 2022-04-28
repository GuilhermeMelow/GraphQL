using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphQL.Extensions
{
    public class JwtAuthenticatorCommand
    {
        private readonly AppSettings settings;
        private readonly HttpContext context;

        public JwtAuthenticatorCommand(AppSettings settings, HttpContext context)
        {
            this.settings = settings;
            this.context = context;
        }

        public void Execute(string token)
        {
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

            AttachUserInContext(claims);
        }

        private void AttachUserInContext(ClaimsPrincipal claims)
        {
            context.Items["user"] = new
            {
                Id = claims.FindFirst(c => c.Type == "id"),
                UserName = claims.FindFirst(c => c.Type == "username")
            };
        }
    }
}
