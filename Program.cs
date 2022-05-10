using GraphQL.Configuration;
using GraphQL.Extensions.Authentication;
using GraphQL.Extensions.UserCache;
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

    services.AddSingleton<ApplicationUserCache>();
    services.AddSingleton<AuthorRepository>();
    services.AddSingleton<BookRepository>();
    services.AddSingleton<IBookAddUseCase, BookAddUseCase>();
    services.AddSingleton<IApplicationUserService, ApplicationUserService>();
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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

    app.UserCacheUser();

    app.UsePlayground();

    app.Run();
}
