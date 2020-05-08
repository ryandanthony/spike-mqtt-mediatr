// <copyright file="IMessageAdapter.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using MQTTnet;

namespace Application.Messages
{
    public interface IMessageAdapter
    {
        string GetTypeName(Type type);
        Type GetType(string type);
        void AddMessage(string key, Type value);
        IMessageConverter  CreateConverter(string contentType, Encoding encoding, string protocol);
    }
}