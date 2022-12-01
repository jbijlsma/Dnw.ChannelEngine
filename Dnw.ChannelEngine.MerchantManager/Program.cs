using Dnw.ChannelEngine.MerchantManager.Clients;
using Dnw.ChannelEngine.MerchantManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMerchantStore, MerchantStore>();
builder.Services.AddHttpClient<DaprActorClient>(config =>
{
    // todo: get from config
    const int daprPort = 5050;
    config.BaseAddress = new Uri($"http://localhost:{daprPort}");
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();