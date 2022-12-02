using System;
using JetBrains.Annotations;

namespace Dnw.ChannelEngine.Messages
{
    public class ChannelProductRefreshStarted : MerchantChannelMessage
    {
        public DateTime StartedAt { [UsedImplicitly] get; set; }
    }
}