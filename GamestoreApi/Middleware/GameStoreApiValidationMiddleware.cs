using System.Text.Json;

namespace GamestoreApi.Middleware
{
    public class GameStoreApiValidationMiddleware(RequestDelegate next)
    {
        private static readonly Dictionary<(string method, string pattern), string[]> _rules = new()
        {
            { ("POST", "/v1/api/GameStoreApi"),      ["name", "genre", "price", "releaseDate"] },
            { ("PUT",  "/v1/api/GameStoreApi/{id}"), ["name", "genre", "price", "releaseDate"] },
        };

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method.ToUpper();
            var path   = context.Request.Path.Value?.ToLower() ?? "";

            var rule = _rules.FirstOrDefault(r =>
                r.Key.method == method && MatchesPattern(path, r.Key.pattern));

            if (rule.Value is not null)
            {
                context.Request.EnableBuffering();

                string body;
                using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                }
                context.Request.Body.Position = 0;

                if (!ValidateBody(body, rule.Value, out var missing))
                {
                    context.Response.StatusCode  = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        error = "Validation failed",
                        missing
                    }));
                    return;
                }
            }

            await next(context);
        }

        private static bool MatchesPattern(string path, string pattern)
        {
            var patternParts = pattern.Split('/');
            var pathParts    = path.Split('/');

            if (patternParts.Length != pathParts.Length) return false;

            return patternParts.Zip(pathParts).All(pair =>
                pair.First.StartsWith('{') || pair.First == pair.Second);
        }

        private static bool ValidateBody(string body, string[] requiredFields, out string[] missing)
        {
            missing = [];

            if (string.IsNullOrWhiteSpace(body))
            {
                missing = requiredFields;
                return false;
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root      = doc.RootElement;

                missing = requiredFields
                    .Where(f =>
                        !root.TryGetProperty(f, out var v) ||
                        v.ValueKind == JsonValueKind.Null ||
                        (v.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(v.GetString())) ||
                        (f == "price" && v.TryGetDecimal(out var price) && price <= 0) ||
                        (f == "releaseDate" && (!v.TryGetDateTime(out var releaseDate) || releaseDate <= DateTime.MinValue)))
                    .ToArray(); 

                return missing.Length == 0;
            }
            catch (JsonException)
            {
                missing = ["<invalid JSON>"];
                return false;
            }
        }
    }
}