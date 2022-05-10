using GraphQL.Configuration;
using GraphQL.Extensions.Authentication;
using GraphQL.Models;
using GraphQL.Mutations;
using GraphQL.Services.UserService;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public async Task<UserResult> RegisterUser([Service] SignInManager<User> signInManager, [Service] IApplicationUserService userService, UserDto userDto)
        {
            await userService.RegisterAsync(userDto);

            return new UserResult
            {
                Username = userDto.Username,
                Token = GerarJwt(userDto.Username)
            };
        }

        public async Task<UserResult> LoginUser([Service] SignInManager<User> signInManager, [Service] IApplicationUserService userService, UserDto userDto)
        {
            await userService.LoginAsync(userDto);

            return new UserResult
            {
                Username = userDto.Username,
                Token = GerarJwt(userDto.Username)
            };
        }

        [Authorize]
        public async Task<string> RedefinirApiKey([Service] ApplicationDbContext context, [Service] IApplicationUserService userService)
        {
            var user = userService.GetUser();

            if (user == null) throw new InvalidOperationException("Unauthorized");

            user.ApiKey = GerarApiKey();
            context.Update(user);
            await context.SaveChangesAsync();

            return user.ApiKey;
        }

        private static string GerarApiKey()
        {
            var buffer = new byte[64];
            var random = new Random();

            random.NextBytes(buffer);

            return Convert.ToBase64String(buffer);
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