using GraphQL.Configuration;
using GraphQL.Repositories;
using GraphQL.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

{
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

    var services = builder.Services;

    services.AddDbContext<ApplicationDbContext>();
    services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager();

    services.AddSingleton<AuthorRepository>();
    services.AddSingleton<BookRepository>();
    services.AddSingleton<IBookAddUseCase, BookAddUseCase>();

    services.AddAutoMapper(typeof(GraphQLProfile));

    services.AddAuthJwt(builder.Configuration);

    services.AddGraphql();

    services.AddCors(c => c.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:8080")
            .AllowAnyHeader()
            .AllowCredentials();
    }));
}

var app = builder.Build();

{
    app.UseWebSockets();

    app.UseCors();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UsePlayground();

    app.Run();
}