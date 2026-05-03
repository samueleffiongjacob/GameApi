using Scalar.AspNetCore;
using GamestoreApi.Service;
using GamestoreApi.Middleware;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddGameStoreApiAuth(builder.Configuration);
builder.Services.AddAuthorization();
//builder.Services.AddScoped<IGameStoreCharacterService, GameStoreService>();
builder.Services.AddSingleton<IGameStoreCharacterService, GameStoreService>();
builder.Services.AddSingleton<IJwtValidator, GameStoreJwtValidator>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
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
