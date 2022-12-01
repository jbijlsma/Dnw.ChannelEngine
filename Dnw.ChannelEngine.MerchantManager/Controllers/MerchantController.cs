using System.Text.Json;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dnw.ChannelEngine.Actors;
using Dnw.ChannelEngine.MerchantManager.Clients;
using Dnw.ChannelEngine.MerchantManager.Services;
using Dnw.ChannelEngine.Messages;
using Dnw.ChannelEngine.Models;
using Microsoft.AspNetCore.Mvc;
using Merchant = Dnw.ChannelEngine.Models.Merchant;
using MerchantChannel = Dnw.ChannelEngine.Models.MerchantChannel;

namespace Dnw.ChannelEngine.MerchantManager.Controllers;

[ApiController]
[Route("[controller]")]
public class MerchantController : ControllerBase
{
    private readonly DaprActorClient _daprActorClient;
    private readonly IMerchantStore _merchantStore;
    
    public MerchantController(DaprActorClient daprActorClient, IMerchantStore merchantStore)
    {
        _daprActorClient = daprActorClient;
        _merchantStore = merchantStore;
    }
    
    [HttpPost("simulation/start")]
    public async Task<IActionResult> StartSimulation(StartSimulation options)
    {
        if (!_merchantStore.IsEmpty()) return Ok();

        var channels = Enumerable.Range(0, options.NumberOfChannels).Select(CreateChannel).ToArray();
        var merchants = Enumerable.Range(0, options.NumberOfMerchants).Select(i => CreateMerchant(i, channels, options)).ToArray();
        
        _merchantStore.Update(merchants);
        
        Console.WriteLine(JsonSerializer.Serialize(merchants));

        var initMerchantTasks = merchants.Select(merchant =>
        {
            var proxy = ActorProxy.Create<IMerchant>(new ActorId(merchant.Id), nameof(Actors.Merchant));
            return proxy.Init(merchant);
        });

        await Task.WhenAll(initMerchantTasks);
        
        Console.WriteLine("Started simulation");
        
        return Ok();
    }
    
    [HttpGet("simulation/stop")]
    public async Task<IActionResult> StopSimulation()
    {
        if (_merchantStore.IsEmpty()) return Ok();
        
        var merchants = _merchantStore.GetAll();
        var stopMerchantTasks = merchants.Select(merchant => 
            ActorProxy.Create<IMerchant>(new ActorId(merchant.Id), nameof(Actors.Merchant)).Stop());

        await Task.WhenAll(stopMerchantTasks);
        
        var deleteMerchantTasks = merchants.Select(merchant => 
            _daprActorClient.Delete(nameof(Actors.Merchant), merchant.Id));
        
        await Task.WhenAll(deleteMerchantTasks);

        _merchantStore.Clear();

        return Ok();
    }

    private static Merchant CreateMerchant(int i, Channel[] channels, StartSimulation options)
    {
        var merchantId = i.ToString();
        var numberOfChannelsToSelect = Random.Shared.Next(options.MinNumberOfChannelsPerMerchant, options.MaxNumberOfChannelsPerMerchant + 1);
        
        return new Merchant
        {
            Id = merchantId,
            Name = $"Merchant_{i}",
            Channels = GetRandomChannels(numberOfChannelsToSelect, channels, merchantId, options)
        };
    }

    private static MerchantChannel[] GetRandomChannels(int n, Channel[] selection, string merchantId, StartSimulation options)
    {
        var selectedChannelIds = new HashSet<string>();
        var randomChannels = Enumerable.Range(0, n).Select(_ => GetRandomChannel(selection, selectedChannelIds)).ToArray();

        return randomChannels.Select((channel, i) => new MerchantChannel
        {
            Id = $"{merchantId}:{i}",
            Name = $"MerchantChannel_{i}",
            RefreshIntervalInSeconds = Random.Shared.Next(options.MinRefreshInternalInSeconds, options.MaxRefreshInternalInSeconds),
            Channel = channel
        }).ToArray();
    }

    private static Channel GetRandomChannel(Channel[] toSelectFrom, HashSet<string> selectedChannelIds)
    {
        var options = toSelectFrom.Where(channel => !selectedChannelIds.Contains(channel.Id)).ToArray();
        var selectedOption = options[Random.Shared.Next(0, options.Length)];

        selectedChannelIds.Add(selectedOption.Id);

        return selectedOption;
    }

    private static Channel CreateChannel(int i)
    {
        return new Channel
        {
            Id = i.ToString(),
            Name = $"ProductFeed_{i}"
        };
    }
}