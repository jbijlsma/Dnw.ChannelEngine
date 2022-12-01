using System;

namespace Dnw.ChannelEngine.Models
{
    public class Channel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
    }
}