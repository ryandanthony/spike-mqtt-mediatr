// <copyright file="MqttInboundResponse.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>
namespace Application.Messages
{
    public class MqttInboundResponse
    {
        public object Message { get; set; }
        public string Topic { get; set; }
        public byte[] Payload { get; set; }
        public byte[] CorrelationData { get; set; }
        public string PayloadType { get; set; }
        public string PayloadVersion { get; set; }
        public string MessageType { get; set; }

        public bool Success { get; set; }
    }
}

