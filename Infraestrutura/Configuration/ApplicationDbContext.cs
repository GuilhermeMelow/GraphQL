using GraphQL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GraphQL.Configuration
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly IConfiguration configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connection = configuration.GetConnectionString("DefaultConnection");

            options.UseSqlServer(connection);
        }
    }
}