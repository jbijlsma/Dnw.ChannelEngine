using System;

namespace Dnw.ChannelEngine.Messages
{
    public class ChannelProductRefreshStarted
    {
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string MerchantChannelName { get; set; }
        public DateTime StartedAt { get; set; }
        public string RunningOn { get; set; }
        public string ActorId { get; set; }
    }
}