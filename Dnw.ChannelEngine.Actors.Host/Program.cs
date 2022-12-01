using Dnw.ChannelEngine.Actors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<Merchant>();
    options.Actors.RegisterActor<MerchantChannel>();
});

var app = builder.Build();

app.MapGet("/", () => "Dnw.ChannelEngine.Actors.Host is running!");

app.MapActorsHandlers();

app.Run();
