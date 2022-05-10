using GraphQL.Extensions.UserCache;
using GraphQL.Models;
using GraphQL.Mutations;
using Microsoft.AspNetCore.Identity;

namespace GraphQL.Services.UserService
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationUserCache userCache;

        public ApplicationUserService(SignInManager<User> signInManager, ApplicationUserCache userCache)
        {
            this.signInManager = signInManager;
            this.userCache = userCache;
        }

        public async Task LoginAsync(UserDto userDto)
        {
            var result = await signInManager.PasswordSignInAsync(userDto.Username, userDto.Password, false, true);

            if (result.IsLockedOut) throw new ArgumentException("O você foi bloqueado por conta das tentativas falhas anteriores.");

            if (!result.Succeeded) throw new ArgumentException("Algum dado pode estar incorreto, verifique por favor.");

            await userCache.DefineCacheAsync(userDto.Username);
        }

        public async Task RegisterAsync(UserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.Username,
                Email = userDto.Username,
                EmailConfirmed = true
            };

            var result = await signInManager.UserManager.CreateAsync(user, userDto.Password);

            if (result.Errors.Any()) throw new ArgumentException(string.Join(";", result.Errors.Select(c => c.Description)));

            await signInManager.SignInAsync(user, false);

            await userCache.DefineCacheAsync(user.Email);
        }

        public User? GetUser()
        {
            return userCache.GetCache();
        }
    }
}
