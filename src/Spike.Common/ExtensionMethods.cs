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


        public static string ToJsonString(this byte[] @value)
        {
            return Encoding.UTF8.GetString(@value);
        }

        public static dynamic ToObject(this byte[] value, Type type)
        {
            return JsonConvert.DeserializeObject(@value.ToString(), type);
        }

        public static T To<T>(this byte[] value)
        {
            var stringVal = @value.ToJsonString();
            return JsonConvert.DeserializeObject<T>(stringVal);
        }

        //DONT TAKE FORWARD, just a helper for spike
        public static MqttApplicationMessageBuilder WithMessage<T>(this MqttApplicationMessageBuilder builder,
            T message)
        {
            return builder.WithPayload(JsonConvert.SerializeObject(message).ToByteArray());
        }
    }
}
