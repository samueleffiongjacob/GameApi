using Scalar.AspNetCore;
using GamestoreApi.Service;
using GamestoreApi.Middleware;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Registers JWT bearer authentication using Jwt:Secret, Jwt:Issuer, and Jwt:Audience.
builder.Services.AddGameStoreApiAuth(builder.Configuration);
builder.Services.AddAuthorization();
//builder.Services.AddScoped<IGameStoreCharacterService, GameStoreService>();
builder.Services.AddSingleton<IGameStoreCharacterService, GameStoreService>();
// Fallback validator used by the custom guard when ASP.NET auth did not set context.User.
builder.Services.AddSingleton<IJwtValidator, GameStoreJwtValidator>();
// Dev token generator for testing (used by POST /v1/api/auth/token).
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    // TODO: Add dev-only token endpoint here, e.g. POST /v1/api/auth/token, for quick local testing.
}
app.UseMiddleware<GameStoreapiExceptionHandlingMiddleware>();
app.UseMiddleware<GameStoreApiRequestLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GameStoreApiGuard>();
app.UseMiddleware<GameStoreApiValidationMiddleware>();

// Get All Games
//app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
