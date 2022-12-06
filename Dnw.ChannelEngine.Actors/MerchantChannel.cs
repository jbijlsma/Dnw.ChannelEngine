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
    private const string ReminderName = "ReminderName";

    private const string StateKey = "state";
    private State? _state;
    
    private string MerchantId => Id.GetId().Split(":").FirstOrDefault()!;
    
    public MerchantChannel(ActorHost host) : base(host)
    {
    }

    public async Task Init(string merchantName, Models.MerchantChannel channel)
    {
        _state = new State(merchantName, channel.Name, channel.RefreshIntervalInSeconds, channel.RefreshTimeInSeconds);
        await StateManager.AddOrUpdateStateAsync(StateKey, _state, (_, _) => _state);

        await RegisterReminder();
        
        await PublishMessage<ChannelProductRefreshScheduled>();
    }

    public async Task Stop()
    {
        await UnregisterReminder();
        await PublishMessage<ChannelProductRefreshStopped>();
    }

    protected override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        await PublishMessage<ChannelProductRefreshStarted>(msg =>
            msg.StartedAt = DateTime.UtcNow);

        await Task.Delay(1000 * _state!.RefreshTimeInSeconds);
        
        await PublishMessage<ChannelProductRefreshFinished>(
            msg => msg.CompletedAt = DateTime.UtcNow);
    }

    private async Task PublishMessage<T>(Action<T>? updateMessage = null) where T : MerchantChannelMessage, new()
    {
        var messageType = typeof(T).Name;
        
        var msg = new T
        {
            MessageType = messageType,
            MerchantId = MerchantId,
            MerchantName = _state?.MerchantName,
            MerchantChannelName = _state?.MerchantChannelName,
            RunningOn = Environment.MachineName,
            ActorId = Id.GetId()
        };

        updateMessage?.Invoke(msg);

        using var client = new DaprClientBuilder().Build();
        await client.PublishEventAsync(PubSubChannels.Queue, messageType, msg);
        await client.PublishEventAsync(PubSubChannels.Broadcast, messageType, msg);
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
            TimeSpan.FromSeconds(Random.Shared.Next(0, _state!.RefreshIntervalInSeconds)),
            TimeSpan.FromSeconds(_state!.RefreshIntervalInSeconds));
    }

    private record State(string MerchantName, string MerchantChannelName, int RefreshIntervalInSeconds, int RefreshTimeInSeconds);
}