using StackExchange.Redis;

namespace Dnw.ChannelEngine.AdminUI;

public class ClearRedisStartupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ClearRedisStartupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // using var scope = _serviceProvider.CreateScope();
        // using var mux = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        //
        // foreach (var server in mux.GetServers())
        // {
        //     server.FlushDatabaseAsync();
        // }
      
        return Task.CompletedTask;
    }
}