using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace Dnw.ChannelEngine.Actors;

public interface IMerchantProductFeed : IActor
{
}

public class MerchantProductFeed : Actor, IMerchantProductFeed, IRemindable
{
    public MerchantProductFeed(ActorHost host) : base(host)
    {
    }

    public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        throw new NotImplementedException();
    }
}