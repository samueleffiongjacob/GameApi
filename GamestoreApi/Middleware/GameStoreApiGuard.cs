namespace GamestoreApi.Middleware
{
    public class GameStoreApiGuard
    {
        private static readonly HashSet<string> BlockedPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/v1/api/GameStoreApi/blocked",
            "/v1/api/GameStoreApi/forbidden",
            "/v1/api/GameStoreApi/badrequest",
            "/v1/api/GameStoreApi/invalidinput"
        };

        private static readonly string[] ExemptPaths = new[]
        {
            "/scalar",
            "/health",
            "/"
        };

        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly ILogger<GameStoreApiGuard> _logger;
        private readonly IJwtValidator _jwtValidator;

        public GameStoreApiGuard(RequestDelegate next, IConfiguration config, ILogger<GameStoreApiGuard> logger, IJwtValidator jwtValidator)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jwtValidator = jwtValidator ?? throw new ArgumentNullException(nameof(jwtValidator));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

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

            // check if Asp.net already authenticated the user
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                context.Response.Headers["X-Request-Guard"] = "ASP-Validated";
                await _next(context);
                return;
            }

            /* ASP.NET did not authenticate — guard tries manually as fallback
               extract bearer token from Authorization header
            */
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers.WWWAuthenticate = "Bearer";
                await context.Response.WriteAsync("Authorization header missing or malformed.");
                return;
            }

            var token = authHeader["Bearer ".Length..].Trim();
            if (!_jwtValidator.TryValidateToken(token, out var principal))
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

        // JWT validation is delegated to the registered IJwtValidator implementation.
    }
}