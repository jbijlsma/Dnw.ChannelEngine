namespace Dnw.ChannelEngine.Models
{
    public class MerchantChannel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int RefreshIntervalInSeconds { get; set; }
        public int RefreshTimeInSeconds { get; set; }
        public Channel Channel { get; set; }
    }
}