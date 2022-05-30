using GraphQL.Models;
using GraphQL.Operations.Mutations.Dtos;

namespace GraphQL.Services.UserService
{
    public interface IApplicationUserService
    {
        Task<User> GetUserAsync(string email);
        Task RegisterAsync(UserDto userDto);
        Task LoginAsync(UserDto userDto);
        Task<string> RedefineApiKey(string userEmail);
    }
}
