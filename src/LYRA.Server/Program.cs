using LYRA.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddLyraApplication();

var app = builder.Build();

app.UseLyraMiddleware(app.Environment);

app.MapLyraEndpoints();

await app.MigrateAndSeedAsync();

app.Run();