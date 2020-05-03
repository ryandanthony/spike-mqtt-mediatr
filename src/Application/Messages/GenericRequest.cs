// <copyright file="GenericRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using MediatR;
using MQTTnet;
using Spike.Common;

namespace Application.Messages
{
    public class GenericRequest<T> : IRequest<GenericResponse>, IGenericRequest
    {
        private readonly Lazy<T> _lazyMessage;

        public GenericRequest()
        {
            PropertyBag = new Dictionary<string, object>();
            _lazyMessage = new Lazy<T>(() => RawMessage.Payload.To<T>());
        }

        public Dictionary<string, object> PropertyBag { get; set; }
        public MqttApplicationMessage RawMessage { get; set; }

        public T Message => _lazyMessage.Value;
    }
}