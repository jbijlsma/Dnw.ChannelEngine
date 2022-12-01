using System;
using System.Collections.Generic;

namespace Dnw.ChannelEngine.Models
{
    public class Merchant
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public ICollection<MerchantChannel> Channels { get; set; } = Array.Empty<MerchantChannel>();
    }
}