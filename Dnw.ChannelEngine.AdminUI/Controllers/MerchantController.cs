using Dapr.Client;
using Dnw.ChannelEngine.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Dnw.ChannelEngine.AdminUI.Controllers;

[ApiController]
[Route("[controller]")]
public class MerchantController : ControllerBase
{
    [HttpGet("simulation/start")]
    public async Task<IActionResult> StartSimulation()
    {
        using var client = new DaprClientBuilder().Build();
        await client.InvokeMethodAsync(HttpMethod.Post, "merchant-manager", "merchant/simulation/start", 
            new StartSimulation
            {
                NumberOfMerchants = 3,
                NumberOfChannels = 3,
                MinNumberOfChannelsPerMerchant = 3,
                MaxNumberOfChannelsPerMerchant = 3
            });

        return Ok();
    }
    
    [HttpGet("simulation/stop")]
    public async Task<IActionResult> StopSimulation()
    {
        using var client = new DaprClientBuilder().Build();
        await client.InvokeMethodAsync(HttpMethod.Post, "merchant-manager", "merchant/simulation/stop");

        return Ok();
    }
}