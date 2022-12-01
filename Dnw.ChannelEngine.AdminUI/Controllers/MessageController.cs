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
    [Topic(PubSubChannels.Default, nameof(ChannelProductRefreshStarted))] 
    public IResult Post(ChannelProductRefreshStarted msg)
    {
        SendSse(msg);
        return Results.Ok();
    }

    [HttpPost($"/{nameof(ChannelProductRefreshFinished)}")]
    [Topic(PubSubChannels.Default, nameof(ChannelProductRefreshFinished))] 
    public IResult Post(ChannelProductRefreshFinished msg)
    {
        SendSse(msg);
        return Results.Ok();
    }
    
    private void SendSse(object msg)
    {
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