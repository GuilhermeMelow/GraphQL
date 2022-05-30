using GraphQL.Extensions;
using GraphQL.Extensions.Authentication.Providers;
using GraphQL.Operations.Mutations.UseCases;
using GraphQL.Services.Repositories;
using GraphQL.Services.UserService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<AuthorRepository>();
            services.AddSingleton<BookRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<AuthenticationProviderFactory>();
            services.AddScoped<JwtAuthenticatorProvider>();
            services.AddScoped<ApiAuthenticatorProvider>();

            services.AddScoped<IBookAddUseCase, BookAddUseCase>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IClaimsTransformation, CustomClaimsTransformer>();
        }
    }
}
