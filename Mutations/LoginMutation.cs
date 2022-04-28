using GraphQL.Configuration;
using GraphQL.Extensions;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public sealed class LoginMutation
    {
        private readonly AppSettings appsettings;

        public LoginMutation(IOptions<AppSettings> appsettings)
        {
            this.appsettings = appsettings.Value;
        }

        public async Task<UserResult> RegisterUser([Service] SignInManager<ApplicationUser> signInManager, UserDto userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.Username,
                Email = userDto.Username,
                EmailConfirmed = true
            };

            var result = await signInManager.UserManager.CreateAsync(user, userDto.Password);

            if (result.Errors.Any())
            {
                throw new ArgumentException(string.Join(";", result.Errors.Select(c => c.Description)));
            }

            await signInManager.SignInAsync(user, false);

            return new UserResult
            {
                Username = userDto.Username,
                Token = GerarJwt(userDto.Username)
            };
        }

        public async Task<UserResult> LoginUser([Service] SignInManager<ApplicationUser> signInManager, UserDto userDto)
        {
            var result = await signInManager.PasswordSignInAsync(userDto.Username, userDto.Password, false, true);

            if (result.IsLockedOut)
            {
                throw new ArgumentException("O você foi bloqueado por conta das tentativas falhas anteriores.");
            }

            if (!result.Succeeded)
            {
                throw new ArgumentException("Algum dado pode estar incorreto, verifique por favor.");
            }

            return new UserResult
            {
                Username = userDto.Username,
                Token = GerarJwt(userDto.Username)
            };
        }

        [Authorize]
        public async Task<string> RedefinirApiKey([Service] ApplicationDbContext context, [Service] SignInManager<ApplicationUser> signInManager)
        {
            if (signInManager.Context.User.Identity == null) throw new InvalidOperationException("Unauthorized");

            var user = context.Users.FirstOrDefault(c => c.Email == signInManager.Context.User.Identity.Name);

            if (user == null) throw new InvalidOperationException("Not user registered");

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
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, userName),
                    new Claim(JwtRegisteredClaimNames.Email, userName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Issuer = appsettings.Emissor,
                Audience = appsettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(appsettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            });

            return tokenHandler.WriteToken(token);
        }
    }

    public class UserDto
    {
        public string Username { get; init; }
        public string Password { internal get; init; }
    }

    public class UserResult
    {
        public string Username { get; init; }
        public string Token { get; init; }
    }
}