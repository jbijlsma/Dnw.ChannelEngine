using System.Text.Json;
using Dapr;
using Dnw.ChannelEngine.Messages;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Mvc;

namespace Dnw.ChannelEngine.AdminUI.Controllers;

[ApiController]
public class MessageController : ControllerBase
{
    private readonly IServerSentEventsService _sseService;

    public MessageController(IServerSentEventsService sseService)
    {
        _sseService = sseService;
    }

    [HttpPost($"/{nameof(ChannelProductRefreshStarted)}")]
    [Topic(PubSubChannels.Broadcast, nameof(ChannelProductRefreshStarted))] 
    public IResult Post(ChannelProductRefreshStarted msg)
    {
        SendSse(msg);
        return Results.Ok();
    }

    [HttpPost($"/{nameof(ChannelProductRefreshFinished)}")]
    [Topic(PubSubChannels.Broadcast, nameof(ChannelProductRefreshFinished))] 
    public IResult Post(ChannelProductRefreshFinished msg)
    {
        SendSse(msg);
        return Results.Ok();
    }
    
    [HttpPost($"/{nameof(ChannelProductRefreshScheduled)}")]
    [Topic(PubSubChannels.Broadcast, nameof(ChannelProductRefreshScheduled))] 
    public IResult Post(ChannelProductRefreshScheduled msg)
    {
        SendSse(msg);
        return Results.Ok();
    }
    
    [HttpPost($"/{nameof(ChannelProductRefreshStopped)}")]
    [Topic(PubSubChannels.Broadcast, nameof(ChannelProductRefreshStopped))] 
    public IResult Post(ChannelProductRefreshStopped msg)
    {
        SendSse(msg);
        return Results.Ok();
    }
    
    // The generics are necessary here for the serialization
    // If we use MerchantChannelMessage instead of T for the parameter
    // only the MerchantChannelMessage properties are serialized (and not the properties
    // of the derived type)
    private void SendSse<T>(T msg) where T : MerchantChannelMessage
    {
        msg.SentByAdminUiMachine = Environment.MachineName;
        var json = JsonSerializer.Serialize(msg, new JsonSerializerOptions
        {
             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
            
        var clients = _sseService.GetClients();
        foreach (var client in clients)
        {
            client.SendEventAsync(json, CancellationToken.None).ConfigureAwait(false);
        }
    }
}