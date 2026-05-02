using GamestoreApi.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<GameStoreService>();

var app = builder.Build();

// Get All Games
app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
