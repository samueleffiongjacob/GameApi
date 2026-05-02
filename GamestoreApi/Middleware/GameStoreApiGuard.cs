namespace GamestoreApi.Middleware
{
    public class GameStoreApiGuard(RequestDelegate next)
    {
        private static readonly HashSet<string> BlockedPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "v1/api/GameStoreApi/blocked",
            "v1/api/GameStoreApi/forbidden",
            "v1/api/GameStoreApi/badrequest",
            "v1/api/GameStoreApi/invalidinput"
        };

        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("v1/api/GameStoreApi"))
            {
                await _next(context);
                return;
            }

            if (BlockedPaths.Contains(context.Request.Path))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Blocked test path.");
                return;
            }

            if (context.Request.Query["secure"].ToString() != "true")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing secure=true.");
                return;
            }

            if (context.Request.Query["token"].ToString() != "valid-token")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            var input = context.Request.Query["input"].ToString();
            if (!IsValidInput(input))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid input.");
                return;
            }

            context.Response.Headers["X-Request-Guard"] = "Enabled";
            await _next(context);
        }

        private static bool IsValidInput(string input)
        {
            // Keep validation simple and predictable for testing.
            return !string.IsNullOrWhiteSpace(input)
                && input.Length <= 100
                && input.All(char.IsLetterOrDigit)
                && !input.Contains("<script>", StringComparison.OrdinalIgnoreCase);
        }
    }
}

