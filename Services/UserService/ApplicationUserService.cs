using GraphQL.Models;
using GraphQL.Operations.Mutations.Dtos;
using Microsoft.AspNetCore.Identity;

namespace GraphQL.Services.UserService
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public ApplicationUserService(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task LoginAsync(UserDto userDto)
        {
            var result = await signInManager.PasswordSignInAsync(userDto.Username, userDto.Password, false, true);

            if (result.IsLockedOut) throw new ArgumentException("O você foi bloqueado por conta das tentativas falhas anteriores.");

            if (!result.Succeeded) throw new ArgumentException("Algum dado pode estar incorreto, verifique por favor.");
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
        }

        public Task<User> GetUserAsync(string email)
        {
            return userManager.FindByEmailAsync(email);
        }

        public async Task<string> RedefineApiKey(string userEmail)
        {
            var user = await GetUserAsync(userEmail);

            if (user == null) throw new InvalidOperationException(message: "Usuário inexistente.");

            user.ApiKey = GenerateApiKey();
            await userManager.UpdateAsync(user);

            return user.ApiKey;
        }

        private static string GenerateApiKey()
        {
            var buffer = new byte[64];
            var random = new Random();

            random.NextBytes(buffer);

            return Convert.ToBase64String(buffer);
        }
    }
}
