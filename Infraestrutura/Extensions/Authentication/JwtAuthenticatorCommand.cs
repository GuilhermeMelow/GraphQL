using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GraphQL.Extensions.Authentication
{
    public class JwtAuthenticatorCommand
    {
        private readonly AppSettings settings;

        public JwtAuthenticatorCommand(AppSettings settings)
        {
            this.settings = settings;
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
        }
    }
}
