// <copyright file="IMessageConverter.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Bct.Common.Workflow.Aggregates;
using MQTTnet;

namespace Application.Messages
{
    public interface IMessageConverter
    {
        T ConvertToObject<T>(byte[] raw) where T: class;
        byte[] ConvertToBytes<T>(T obj) where T: class;
    }
}