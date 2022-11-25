using Dapr.Actors;
using Dapr.Actors.Runtime;
using JetBrains.Annotations;

namespace Dnw.ChannelEngine.Actors;

public interface IMerchant : IActor
{
}

[UsedImplicitly]
public class Merchant : Actor, IMerchant, IRemindable
{
    public Merchant(ActorHost host) : base(host)
    {
    }

    public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        throw new NotImplementedException();
    }
}