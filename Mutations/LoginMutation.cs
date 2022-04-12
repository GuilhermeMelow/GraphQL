using GraphQL.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public sealed class LoginMutation
    {
        private readonly AppSettings appsettings;

        public LoginMutation(SignInManager<IdentityUser> signInManager,
                             IOptions<AppSettings> appsettings)
        {
            this.appsettings = appsettings.Value;
        }

        public async Task<UserResult> RegisterUser([Service] SignInManager<IdentityUser> signInManager, UserDto userDto)
        {
            var user = new IdentityUser(userDto.Username)
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
                Token = GerarJwt()
            };
        }

        public async Task<UserResult> LoginUser([Service] SignInManager<IdentityUser> signInManager, UserDto userDto)
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
                Token = GerarJwt()
            };
        }

        private string GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appsettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
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