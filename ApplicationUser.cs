using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string? ApiKey { get; set; }
}