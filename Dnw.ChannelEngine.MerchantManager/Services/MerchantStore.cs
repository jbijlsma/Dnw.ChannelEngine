using Dnw.ChannelEngine.Models;

namespace Dnw.ChannelEngine.MerchantManager.Services;

public interface IMerchantStore
{
    bool IsEmpty();
    Merchant[] GetAll();
    void Clear();
    void Update(Merchant[] merchants);
    bool Exists(string merchantId);
}

public class MerchantStore : IMerchantStore
{
    private Merchant[] _merchants = Array.Empty<Merchant>();

    public bool IsEmpty()
    {
        return !_merchants.Any();
    }

    public Merchant[] GetAll()
    {
        return _merchants;
    }
    
    public void Clear()
    {
        _merchants = Array.Empty<Merchant>();
    }

    public void Update(Merchant[] merchants)
    {
        _merchants = merchants;
    }

    public bool Exists(string merchantId)
    {
        return _merchants.Any(m => m.Id == merchantId);
    }
}