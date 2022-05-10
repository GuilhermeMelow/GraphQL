using GraphQL.Models;
using Microsoft.AspNetCore.Identity;

namespace GraphQL.Extensions.UserCache
{
    public class ApplicationUserCache
    {
        private readonly UserManager<User> userManager;
        private User? _userCache;

        public bool HasCache => _userCache != null;

        public ApplicationUserCache(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task DefineCacheAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return;

            var user = await userManager.FindByEmailAsync(email);

            if (user != null) _userCache = user;
        }

        public async Task UpdateCache(User user)
        {
            if (await userManager.FindByIdAsync(user.Id) == null) return;

            _userCache = user;
        }

        public User? GetCache()
        {
            return _userCache;
        }
    }
}
