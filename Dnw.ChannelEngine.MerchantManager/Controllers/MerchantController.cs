using System.Text.Json;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dnw.ChannelEngine.Actors;
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
    private readonly IMerchantStore _merchantStore;
    
    public MerchantController(IMerchantStore merchantStore)
    {
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
    
    // Awaiting the tasks will cause timeouts with actors that perform long
    // running tasks
    [HttpGet("simulation/stop")]
    public IActionResult StopSimulation()
    {
        if (_merchantStore.IsEmpty()) return Ok();
        
        var merchants = _merchantStore.GetAll();
        merchants.ToList().ForEach(merchant => 
            ActorProxy.Create<IMerchant>(new ActorId(merchant.Id), nameof(Actors.Merchant)).Stop());

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

        return randomChannels.Select((channel, i) =>
        {
            var refreshIntervalInSeconds = Random.Shared.Next(options.MinRefreshInternalInSeconds, options.MaxRefreshInternalInSeconds + 1);
            var refreshTimeInSeconds = Random.Shared.Next(options.MinRefreshTimeInSeconds, options.MaxRefreshTimeInSeconds + 1);
            
            return new MerchantChannel
            {
                Id = $"{merchantId}:{i}",
                Name = $"MerchantChannel_{i}",
                RefreshIntervalInSeconds = refreshIntervalInSeconds,
                RefreshTimeInSeconds = refreshTimeInSeconds,
                Channel = channel
            };
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