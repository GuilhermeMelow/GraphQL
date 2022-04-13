using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GraphQL.Extensions
{
    public class AuthenticationInterceptor : DefaultHttpRequestInterceptor
    {
        private readonly AppSettings appSettings;
        public AuthenticationInterceptor(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override ValueTask OnCreateAsync(HttpContext context, IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder, CancellationToken cancellationToken)
        {
            var aut = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(aut))
            {
                var token = aut.Split(" ")[1];

                try
                {
                    AttachUserToContext(context, token);
                }
                catch (Exception ex)
                {
                    return ValueTask.FromException(ex);
                }
            }

            return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true
            }, out SecurityToken securityToken);

            var jwtToken = (JwtSecurityToken)securityToken;
            var claims = jwtToken.Claims.ToList();

            context.Items["user"] = new
            {
                Id = claims.First(x => x.Type == "id").Value,
                Username = claims.First(x => x.Type == "email").Value,
            };
        }
    }
}
