using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GamestoreApi.Middleware
{
    public class GameStoreApiGuard(RequestDelegate next, IConfiguration config)
    {
        private static readonly HashSet<string> BlockedPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/v1/api/GameStoreApi/blocked",
            "/v1/api/GameStoreApi/forbidden",
            "/v1/api/GameStoreApi/badrequest",
            "/v1/api/GameStoreApi/invalidinput"
        };

        private static readonly string[] ExemptPaths =
        [
            "/scalar",
            "/health",
            "/"
        ];

        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString();

            // pass through exempt paths — docs, health checks, root
            if (ExemptPaths.Any(e => path.StartsWith(e, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // only guard your API routes
            if (!context.Request.Path.StartsWithSegments("/v1/api/GameStoreApi"))
            {
                await _next(context);
                return;
            }

            // block known bad paths before anything else
            if (BlockedPaths.Contains(path))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Resource not found.");
                return;
            }

            // extract bearer token from Authorization header
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers.WWWAuthenticate = "Bearer";
                await context.Response.WriteAsync("Authorization header missing or malformed.");
                return;
            }

            var token = authHeader["Bearer ".Length..].Trim();
            if (!ValidateJwtToken(token, config, out var principal))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid or expired token.");
                return;
            }

            // attach claims to context so controllers can read them
            context.User = principal!;
            context.Response.Headers["X-Request-Guard"] = "Enabled";

            await _next(context);
        }

        private static bool ValidateJwtToken(
            string token,
            IConfiguration config,
            out System.Security.Claims.ClaimsPrincipal? principal)
        {
            principal = null;

            var secret = config["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret)) return false;

            try
            {
                var key       = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var handler   = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = key,
                    ValidateIssuer           = true,
                    ValidIssuer              = config["Jwt:Issuer"],
                    ValidateAudience         = true,
                    ValidAudience            = config["Jwt:Audience"],
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero  // no grace period on expiry
                };

                principal = handler.ValidateToken(token, parameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}