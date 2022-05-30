using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GraphQL.Extensions.Authentication.Providers
{
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

            if (!tokenHandler.CanReadToken(token)) throw new AuthenticationException(message: "Irregular format Token.");

            var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true
            }, out _);

            if (claims == null) throw new AuthenticationException();
        }
    }
}
