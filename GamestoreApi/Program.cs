using Scalar.AspNetCore;
using GamestoreApi.Service;
using GamestoreApi.Middleware;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
//builder.Services.AddScoped<IGameStoreCharacterService, GameStoreService>();
builder.Services.AddSingleton<IGameStoreCharacterService, GameStoreService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseMiddleware<GameStoreapiExceptionHandlingMiddleware>();
app.UseMiddleware<GameStoreApiRequestLoggingMiddleware>();
app.UseMiddleware<GameStoreApiGuard>();
app.UseMiddleware<GameStoreApiValidationMiddleware>();

// Get All Games
//app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
