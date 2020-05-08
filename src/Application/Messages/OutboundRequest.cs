// <copyright file="MqttOutboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;

namespace Application.Messages
{
    public class OutboundRequest<T> : IRequest
    {
        public string Topic { get; set; }
        public object Message { get; set; }
        public byte[] Payload { get; set; }
        public string PayloadType { get; set; }
        public string PayloadVersion { get; set; }
        public string MessageType { get; set; }
        public byte[] CorrelationData { get; set; }
    }
    public class OutboundRequest : IRequest
    {
        public string Topic { get; set; }
        public object Message { get; set; }
        public byte[] Payload { get; set; }
        public string PayloadType { get; set; }
        public string PayloadVersion { get; set; }
        public string MessageType { get; set; }
        public byte[] CorrelationData { get; set; }
    }

}