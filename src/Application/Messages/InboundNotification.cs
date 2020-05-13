// <copyright file="InboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Bct.Common.Workflow.Aggregates;
using MediatR;
using MQTTnet;
using Spike.Common;

namespace Application.Messages
{
    public class InboundNotification<T> : INotification, IInboundNotification<T>
    {
        private readonly Lazy<T> _lazyMessage;

        public InboundNotification()
        {
            PropertyBag = new Dictionary<string, object>();
            _lazyMessage = new Lazy<T>(() =>
            {
                var json = Encoding.UTF8.GetString(RawMessage.Payload);
                var obj = BaseAggregate.Deserialize<T>(json);
                return obj;
            });
        }

        public Dictionary<string, object> PropertyBag { get; set; }
        public MqttApplicationMessage RawMessage { get; set; }

        public T Message => _lazyMessage.Value;
    }

}