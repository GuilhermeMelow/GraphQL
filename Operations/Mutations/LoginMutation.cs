using GraphQL.Extensions.Authentication;
using GraphQL.Operations.Mutations.Dtos;
using GraphQL.Services.UserService;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphQL.Operations.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public sealed class LoginMutation
    {
        private readonly AppSettings appsettings;

        public LoginMutation(IOptions<AppSettings> appsettings)
        {
            this.appsettings = appsettings.Value;
        }

        public async Task<UserResult> RegisterUser([Service] IApplicationUserService userService, UserDto userDto)
        {
            await userService.RegisterAsync(userDto);

            return new UserResult(userDto.Username, GerarJwt(userDto.Username));
        }

        public async Task<UserResult> LoginUser([Service] IApplicationUserService userService, UserDto userDto)
        {
            await userService.LoginAsync(userDto);

            return new UserResult(userDto.Username, Token: GerarJwt(userDto.Username));
        }

        [Authorize]
        public async Task<string> RedefineApiKey([Service] IApplicationUserService userService, [Service] IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            var userEmail = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            return await userService.RedefineApiKey(userEmail);
        }

        private string GerarJwt(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appsettings.Secret);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = appsettings.Emissor,
                Audience = appsettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(appsettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            });

            return tokenHandler.WriteToken(token);
        }
    }
}