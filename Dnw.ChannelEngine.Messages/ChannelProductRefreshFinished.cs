using System;
using JetBrains.Annotations;

namespace Dnw.ChannelEngine.Messages
{
    public class ChannelProductRefreshFinished : MerchantChannelMessage
    {
        public DateTime CompletedAt { [UsedImplicitly] get; set; }
    }
}