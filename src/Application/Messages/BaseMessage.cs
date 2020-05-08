using MQTTnet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Messages
{
    public class BaseMessage
    {
        public string Topic { get; set; }
        public MqttApplicationMessage RawMessage { get; set; }
        public BaseMessage(IMessageConverter typeConverter)
        {
            RawMessage = new MqttApplicationMessage();
            TypeConverter = typeConverter;
        }
        public object Message
        {
            get
            {
                return TypeConverter.ConvertToObject<object>(RawMessage.Payload);
            }
            set
            {
                RawMessage.Payload = TypeConverter.ConvertToBytes<object>(value);
            }
        }
        public byte[] Payload
        {
            get
            {
                return RawMessage.Payload;
            }
        }
        public string PayloadType { get; set; }
        public string PayloadVersion { get; set; }
        public byte[] CorrelationData { get; set; }
        public IMessageConverter TypeConverter { get; private set; }
    }

    public class BaseMessage<T> where T: class
    {
        private readonly Lazy<T> _lazyMessage;
        public Dictionary<string, object> PropertyBag { get; set; }
        public MqttApplicationMessage RawMessage { get; set; }
        public BaseMessage()
        {
            PropertyBag = new Dictionary<string, object>();
            _lazyMessage = new Lazy<T>(() =>
            {
                var obj = TypeConverter.ConvertToObject<T>(RawMessage.Payload);
                return obj;
            });
        }
        public T Message
        {
            get
            {
                return _lazyMessage.Value;
            }
            set
            {
                RawMessage.Payload = TypeConverter.ConvertToBytes<T>(value);
            }
        }
        public IMessageConverter TypeConverter { get; set; }
    }
}
