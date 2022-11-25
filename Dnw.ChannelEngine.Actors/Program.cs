using Dnw.ChannelEngine.Actors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<Merchant>();
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapActorsHandlers();

app.Run();
