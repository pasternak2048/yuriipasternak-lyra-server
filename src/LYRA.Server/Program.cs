using LYRA.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLyraApplication(builder.Configuration);

var app = builder.Build();

await app.UseLyraMiddleware();

app.Run();