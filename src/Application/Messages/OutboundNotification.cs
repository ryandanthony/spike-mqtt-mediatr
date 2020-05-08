// <copyright file="OutboundNotification.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;
using MQTTnet;

namespace Application.Messages
{
    public class OutboundNotification : BaseMessage, INotification
    {
        public OutboundNotification(IMessageConverter typeConverter): base(typeConverter)
        {
            RawMessage = new MqttApplicationMessage();
        }
    }
}