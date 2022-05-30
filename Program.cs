using GraphQL.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

    var services = builder.Services;

    services.AddDependencies();

    services.AddAutoMapper(typeof(GraphQLProfile));

    services.AddAuthentication(builder.Configuration);

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

    app.UseIdentityConfig();

    app.UsePlayground();

    app.Run();
}
