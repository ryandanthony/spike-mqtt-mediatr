// <copyright file="IMqttOutboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;

namespace Application.Messages
{
    public interface IMqttOutboundRequest : IRequest<MqttOutboundResponse>
    {
        string Topic { get; }
        object GetMessage();
        string GetMessageType();
    }
}