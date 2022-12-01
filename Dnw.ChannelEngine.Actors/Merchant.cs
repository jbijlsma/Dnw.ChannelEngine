using Dapr.Actors;
using Dapr.Actors.Runtime;
using JetBrains.Annotations;

namespace Dnw.ChannelEngine.Actors;

public interface IMerchant : IActor
{
    Task Init(Models.Merchant merchant);
    Task Stop();
}

[UsedImplicitly]
public class Merchant : Actor, IMerchant
{
    private const string StateKey = "state";
    private State? _state;

    private string MerchantId => Id.GetId();
    
    public Merchant(ActorHost host) : base(host)
    {
    }

    public async Task Init(Models.Merchant merchant)
    {
        if (_state != null) return;

        var channelIds = merchant.Channels.Select(c => c.Id).ToArray();
        _state = new State(channelIds);
        await StateManager.AddOrUpdateStateAsync(StateKey, _state, (_, _) => _state);
        
        var initChannels = merchant.Channels.Select(merchantChannel =>
        {
            var channelActor = ProxyFactory.CreateActorProxy<IMerchantChannel>(GetMerchantScopedActorId(merchantChannel.Id),nameof(MerchantChannel));
            return channelActor.Init(merchant.Name, merchantChannel);
        });
        
        await Task.WhenAll(initChannels);
    }

    public async Task Stop()
    {
        if (_state == null) return;
        
        var stopChannels = _state.MerchantChannelIds.Select(merchantChannelId =>
        {
            var channelActor = ProxyFactory.CreateActorProxy<IMerchantChannel>(GetMerchantScopedActorId(merchantChannelId), nameof(MerchantChannel));
            return channelActor.Stop();
        });
        
        await Task.WhenAll(stopChannels);

       await StateManager.RemoveStateAsync(StateKey);
       _state = null;
    }

    private ActorId GetMerchantScopedActorId(string id)
    {
        return new ActorId($"{MerchantId}:{id}");
    }
    
    private record State(string[] MerchantChannelIds);
}