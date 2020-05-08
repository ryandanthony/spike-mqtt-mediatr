// <copyright file="IMqttOutboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;

namespace Application.Messages
{
    public interface IMqttOutboundRequest : IRequest<MqttOutboundResponse>
    {
        string Topic { get; }
        object Message { get; set; }
        string PayloadType { get; set; }
        string PayloadVersion { get; set; }
        string MessageType { get; set; }
    }
}