using System;
using System.Text;
using MQTTnet;
using Newtonsoft.Json;

namespace Spike.Common
{
    public static class ExtensionMethods
    {
        public static byte[] ToByteArray(this string @value)
        {
            return Encoding.UTF8.GetBytes(@value);
        }


        //DONT TAKE FORWARD, just a helper for spike
        public static MqttApplicationMessageBuilder WithMessage<T>(this MqttApplicationMessageBuilder builder,
            T message)
        {
            return builder.WithPayload(JsonConvert.SerializeObject(message).ToByteArray());
        }
    }
}
