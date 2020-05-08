// <copyright file="InboundNotification.cs" company="TerumoBCT">
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
    public class InboundNotification<T> : BaseMessage<T>, INotification, IInboundNotification<T> where T: class
    {
    }

}