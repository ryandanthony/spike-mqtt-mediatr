// <copyright file="MqttOutboundRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;

namespace Application.Messages
{
    public class MqttOutboundRequest : IMqttOutboundRequest
    {
        public object Message { get; set; }
        public string Topic { get; set; }

        public object GetMessage()
        {
            return Message;
        }

        public string GetMessageType()
        {
            return Message.GetType().Name;
        }
    }

    public class MqttOutboundResponse
    {
        public bool Success { get; set; }
    }
}