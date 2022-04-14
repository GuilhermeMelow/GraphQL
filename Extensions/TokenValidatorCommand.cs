using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphQL.Extensions
{
    public class TokenValidatorCommand
    {
        private readonly AppSettings settings;

        public TokenValidatorCommand(AppSettings settings)
        {
            this.settings = settings;
        }

        public ClaimsPrincipal? Execute(string token)
        {
            var key = Encoding.ASCII.GetBytes(settings.Secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true
            }, out _);
        }
    }
}
