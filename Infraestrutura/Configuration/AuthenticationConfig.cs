﻿using GraphQL.Configuration.ApiKey;
using GraphQL.Extensions.Authentication;
using GraphQL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GraphQL.Configuration
{
    public static class IdentityConfig
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();

            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "smart";

                })
                .AddPolicyScheme("smart", "Authorization Selector", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                        if (string.IsNullOrEmpty(authHeader) || authHeader.StartsWith("Bearer ")) return JwtBearerDefaults.AuthenticationScheme;

                        return ApiKeyAuthNDefaults.AuthenticationScheme;
                    };
                })
                .AddApiKey(options =>
                {
                    options.ClaimsIssuer = "API-Issuer";
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = appSettings.ValidoEm,
                        ValidIssuer = appSettings.Emissor
                    };
                });

            services.AddAuthorization();
        }

        public static void UseIdentityConfig(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
