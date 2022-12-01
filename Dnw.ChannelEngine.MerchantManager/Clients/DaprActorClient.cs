namespace Dnw.ChannelEngine.MerchantManager.Clients;

public class DaprActorClient
{
    private readonly HttpClient _httpClient;

    public DaprActorClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task Delete(string actorType, string actorId)
    {
        var uri = $"/actors/{actorType}/{actorId}";
        await _httpClient.DeleteAsync(uri);
    }
}