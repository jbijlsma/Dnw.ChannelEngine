namespace Dnw.ChannelEngine.Messages
{
    public class StartSimulation
    {
        public int NumberOfMerchants { get; set; } = 1;
        public int NumberOfChannels { get; set; } = 1;
        public int MinNumberOfChannelsPerMerchant { get; set; } = 1;
        public int MaxNumberOfChannelsPerMerchant { get; set; } = 1;
        public int MinRefreshInternalInSeconds { get; set; } = 20;
        public int MaxRefreshInternalInSeconds { get; set; } = 40;
    }
}