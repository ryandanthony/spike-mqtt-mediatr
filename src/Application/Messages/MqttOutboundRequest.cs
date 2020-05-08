// <copyright file="MqttOutboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;

namespace Application.Messages
{
    public class MqttOutboundRequest : IMqttOutboundRequest
    {
        public object Message { get; set; }
        public string Topic { get; set; }
        public byte[] Payload { get; set; }
        public byte[] CorrelationData { get; set; }
        public string PayloadType { get; set; }
        public string PayloadVersion { get; set; }
        public string MessageType { get; set; }
    }

    public class MqttOutboundResponse
    {
        public bool Success { get; set; }
    }
}