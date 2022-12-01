using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using Dnw.ChannelEngine.Messages;
using JetBrains.Annotations;

namespace Dnw.ChannelEngine.Actors;

public interface IMerchantChannel : IActor
{
    Task Init(string merchantName, Models.MerchantChannel channel);
    Task Stop();
}

[UsedImplicitly]
public class MerchantChannel : Actor, IMerchantChannel, IRemindable
{
    private const int RefreshTimeInSeconds = 10;
    private const string ReminderName = "ReminderName";
    
    private const string StateKey = "state";
    private State? _state;
    
    private string MerchantId => Id.GetId().Split(":").FirstOrDefault()!;
    
    public MerchantChannel(ActorHost host) : base(host)
    {
    }

    public async Task Init(string merchantName, Models.MerchantChannel channel)
    {
        _state = new State(merchantName, channel.Name, channel.RefreshIntervalInSeconds);
        await StateManager.AddOrUpdateStateAsync(StateKey, _state, (_, _) => _state);

        await RegisterReminder();
    }

    public async Task Stop()
    {
        await UnregisterReminder();
    }

    protected override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        await PublishRefreshStartedMessage();

        await Task.Delay(1000 * RefreshTimeInSeconds);
        
        await PublishRefreshFinishedMessage();
    }

    private async Task PublishRefreshStartedMessage()
    {
        var message = new ChannelProductRefreshStarted
        {
            MerchantId = MerchantId,
            MerchantName = _state?.MerchantName,
            MerchantChannelName = _state?.MerchantChannelName,
            StartedAt = DateTime.UtcNow,
            RunningOn = Environment.MachineName,
            ActorId = Id.GetId()
        };
        using var client = new DaprClientBuilder().Build();
        await client.PublishEventAsync(PubSubChannels.Default, nameof(ChannelProductRefreshStarted), message);
    }
    
    private async Task PublishRefreshFinishedMessage()
    {
        var message = new ChannelProductRefreshFinished
        {
            MerchantId = MerchantId,
            MerchantName = _state?.MerchantName,
            MerchantChannelName = _state?.MerchantChannelName,
            CompletedAt = DateTime.UtcNow,
            ActorId = Id.GetId()
        };
        using var client = new DaprClientBuilder().Build();
        await client.PublishEventAsync(PubSubChannels.Default, nameof(ChannelProductRefreshFinished), message);
    }
    
    private async Task UnregisterReminder()
    {
        await UnregisterReminderAsync(ReminderName);
    }

    private async Task RegisterReminder()
    {
        await RegisterReminderAsync(
            ReminderName, 
            Array.Empty<byte>(), 
            TimeSpan.FromSeconds(Random.Shared.Next(0, RefreshTimeInSeconds)),
            TimeSpan.FromSeconds(_state!.RefreshIntervalInSeconds));
    }

    private record State(string MerchantName, string MerchantChannelName, int RefreshIntervalInSeconds);
}