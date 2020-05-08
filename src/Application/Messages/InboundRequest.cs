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
    public class InboundRequest<T> : BaseMessage<T>, IRequest<OutboundNotification>, IInboundRequest<T> 
        where T : class
    {
        public string ResponseTopic { get; set; }
        public byte[] CorrelationData { get; set; }
    }

}