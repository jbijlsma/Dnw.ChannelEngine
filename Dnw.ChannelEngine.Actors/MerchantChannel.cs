using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace Dnw.ChannelEngine.Actors;

public interface IMerchantChannel : IActor
{
}

public class MerchantChannel : Actor, IMerchantChannel, IRemindable
{
    public MerchantChannel(ActorHost host) : base(host)
    {
    }

    public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        throw new NotImplementedException();
    }
}