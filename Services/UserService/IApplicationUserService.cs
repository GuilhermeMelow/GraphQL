using GraphQL.Models;
using GraphQL.Mutations;

namespace GraphQL.Services.UserService
{
    public interface IApplicationUserService
    {
        User? GetUser();
        Task RegisterAsync(UserDto userDto);
        Task LoginAsync(UserDto userDto);
    }
}
