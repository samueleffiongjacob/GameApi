namespace GamestoreApi.Middleware
{
    public class GameStoreApiRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GameStoreApiRequestLoggingMiddleware> _logger;

        public GameStoreApiRequestLoggingMiddleware(RequestDelegate next, ILogger<GameStoreApiRequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Request {Method} {Path}", context.Request.Method, context.Request.Path);

            await _next(context);

            _logger.LogInformation("Response {StatusCode} for {Path}", context.Response.StatusCode, context.Request.Path);
        }

    }
}