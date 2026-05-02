namespace GamestoreApi.Middleware
{
    public class GameStoreapiExceptionHandlingMiddleware(RequestDelegate next, ILogger<GameStoreapiExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GameStoreapiExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception for {Path}", context.Request.Path);

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }

    }
}
