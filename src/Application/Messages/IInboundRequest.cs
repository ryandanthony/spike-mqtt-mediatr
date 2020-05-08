// <copyright file="IInboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using MQTTnet;

namespace Application.Messages
{
    public interface IInboundRequest
    {
        Dictionary<string, object>  PropertyBag { get; set; }
        MqttApplicationMessage RawMessage { get; set; }
        string ResponseTopic { get; set; }
        byte[] CorrelationData { get; set; }
        IMessageConverter TypeConverter { get; set; }
    }

    public interface IInboundRequest<T> : IInboundRequest
    {
    }
}