using Microsoft.AspNetCore.Identity;

namespace GraphQL.Models
{
    public class User : IdentityUser
    {
        public string? ApiKey { get; set; }
    }
}