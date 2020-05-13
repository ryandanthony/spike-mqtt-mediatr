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
    public class InboundRequest<T> : IRequest<InboundResponse>, IInboundRequest<T> 
        where T : class
    {
        private readonly Lazy<T> _lazyMessage;

        public Func<byte[], object> TypeConverter { get; set; }

        public InboundRequest()
        {
            PropertyBag = new Dictionary<string, object>();
            _lazyMessage = new Lazy<T>(() =>
            {
                var obj = TypeConverter(RawMessage.Payload) as T;
                return obj;
            });
        }

        public Dictionary<string, object> PropertyBag { get; set; }
        public MqttApplicationMessage RawMessage { get; set; }
        public string ResponseTopic { get; set; }
        public byte[] CorrelationData { get; set; }

        public T Message => _lazyMessage.Value;
    }

}