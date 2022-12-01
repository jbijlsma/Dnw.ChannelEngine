using System;

namespace Dnw.ChannelEngine.Messages
{
    public class ChannelProductRefreshFinished
    {
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string MerchantChannelName { get; set; }
        public DateTime CompletedAt { get; set; }
        public string ActorId { get; set; }
    }
}