using System.Text.Json;
using Dnw.ChannelEngine.Models;
using StackExchange.Redis;

namespace Dnw.ChannelEngine.MerchantManager.Services;

public interface IMerchantStore
{
    bool IsEmpty();
    IEnumerable<string> GetMerchantIds();
    void Clear();
    void Update(IEnumerable<Merchant> merchants);
    bool Exists(string merchantId);
}

public class MerchantStore : IMerchantStore
{
    private readonly IDatabase _db;

    public MerchantStore(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }
    
    public bool IsEmpty()
    {
        return !_db.KeyExists(GetRedisKey());
    }

    public IEnumerable<string> GetMerchantIds()
    {
        return GetMerchantIdHashSet();
    }

    public void Clear()
    {
        _db.KeyDelete(GetRedisKey());
    }

    public void Update(IEnumerable<Merchant> merchants)
    {
        var merchantIds = new HashSet<string>(merchants.Select(m => m.Id));
        var json = JsonSerializer.Serialize(merchantIds);
        _db.StringSet(GetRedisKey(), json);
    }

    public bool Exists(string merchantId)
    {
        return GetMerchantIdHashSet().Contains(merchantId);
    }

    private HashSet<string> GetMerchantIdHashSet()
    {
        var json = _db.StringGet(new RedisKey("Simulation:Merchants"));
        return JsonSerializer.Deserialize<HashSet<string>>(json!)!;
    }
    
    private static RedisKey GetRedisKey()
    {
        return new RedisKey("Simulation:Merchants");
    }
}