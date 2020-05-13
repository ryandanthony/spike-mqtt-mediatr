// <copyright file="IInboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System.Collections.Generic;
using MQTTnet;

namespace Application.Messages
{
    public interface IInboundNotification
    {
        Dictionary<string, object> PropertyBag { get; set; }
        MqttApplicationMessage RawMessage { get; set; }
    }

    public interface IInboundNotification<T> : IInboundNotification
    {
    }
}