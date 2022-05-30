using GraphQL.Configuration;
using GraphQL.Extensions.Authentication;
using GraphQL.Models;
using GraphQL.Operations.Mutations.UseCases;
using GraphQL.Services.Repositories;
using GraphQL.Services.UserService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
    services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager();

    services.AddSingleton<AuthorRepository>();
    services.AddSingleton<BookRepository>();
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    services.AddSingleton<AuthenticationProviderFactory>();
    services.AddSingleton<IAuthenticationProvider, JwtAuthenticatorProvider>();
    services.AddSingleton<IAuthenticationProvider, ApiAuthenticatorProvider>();

    services.AddScoped<IBookAddUseCase, BookAddUseCase>();
    services.AddScoped<IApplicationUserService, ApplicationUserService>();
    services.AddScoped<IClaimsTransformation, CustomClaimsTransformer>();

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
