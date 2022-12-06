using JetBrains.Annotations;

namespace Dnw.ChannelEngine.Messages
{
    public abstract class MerchantChannelMessage
    {
        public string MessageType { get; set; }
        public string MerchantId { get; set; }
        public string MerchantName { [UsedImplicitly] get; set; }
        public string MerchantChannelName { [UsedImplicitly] get; set; }
        public string RunningOn { [UsedImplicitly] get; set; }
        public string ActorId { get; set; }
        public string SentByAdminUiMachine { get; set; }
    }
}