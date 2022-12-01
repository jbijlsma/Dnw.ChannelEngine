using Dapr;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dnw.ChannelEngine.Actors;
using Dnw.ChannelEngine.MerchantManager.Services;
using Dnw.ChannelEngine.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Dnw.ChannelEngine.MerchantManager.Controllers;

[ApiController]
public class MessageController : ControllerBase
{
    private readonly IMerchantStore _merchantStore;

    public MessageController(IMerchantStore merchantStore)
    {
        _merchantStore = merchantStore;
    }
    
    [HttpPost($"/{nameof(ChannelProductRefreshStarted)}")]
    [Topic(PubSubChannels.Default, nameof(ChannelProductRefreshStarted))] 
    public async Task<IResult> Post(ChannelProductRefreshStarted msg)
    {
        await StopActorIfDoesNotExists(msg.MerchantId, msg.ActorId);
        return Results.Ok();
    }

    [HttpPost($"/{nameof(ChannelProductRefreshFinished)}")]
    [Topic(PubSubChannels.Default, nameof(ChannelProductRefreshFinished))] 
    public async Task<IResult> Post(ChannelProductRefreshFinished msg)
    {
        await StopActorIfDoesNotExists(msg.MerchantId, msg.ActorId);
        return Results.Ok();
    }

    private async Task StopActorIfDoesNotExists(string merchantId, string actorId)
    {
        if (_merchantStore.Exists(merchantId)) return;
        if (string.IsNullOrWhiteSpace(actorId)) return;
        
        var proxy = ActorProxy.Create<IMerchantChannel>(new ActorId(actorId), nameof(MerchantChannel));
        await proxy.Stop();
    }
}