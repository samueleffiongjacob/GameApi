// Middleware/GameStoreApiAuthConfiguration.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GamestoreApi.Middleware
{
    public static class GameStoreApiAuthConfiguration
    {
        public static IServiceCollection AddGameStoreApiAuth(
            this IServiceCollection services,
            IConfiguration config)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!)),
                        ValidateIssuer           = true,
                        ValidIssuer              = config["Jwt:Issuer"],
                        ValidateAudience         = true,
                        ValidAudience            = config["Jwt:Audience"],
                        ValidateLifetime         = true,
                        ClockSkew                = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            ctx.Response.Headers["X-Auth-Failed"] = "InvalidToken";
                            return Task.CompletedTask;
                        },
                        OnChallenge = ctx =>
                        {
                            // suppress default ASP.NET 401
                            // let guard handle it instead
                            ctx.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}