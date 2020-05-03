using System;

namespace Spike.Messages
{
    public class Status
    {
        public string Value { get; set; }
        public DateTimeOffset When { get; set; }
        public Guid MessageId { get; set; }
        public string DeviceId { get; set; }
    }
}
