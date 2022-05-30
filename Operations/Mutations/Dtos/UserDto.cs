namespace GraphQL.Operations.Mutations.Dtos
{
    public record UserDto(string Username, string Password);

    public record UserResult(string UserName, string Token);
}
