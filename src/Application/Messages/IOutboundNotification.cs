// <copyright file="IOutboundNotification.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;

namespace Application.Messages
{
    public interface IOutboundNotification : INotification
    {
        string Topic { get; set; }
        object Message { get; set; }
        byte[] Payload { get; set; }
        string PayloadType { get; set; }
        string PayloadVersion { get; set; }
        string MessageType { get; set; }
        byte[] CorrelationData { get; set; }
    }
}