// <copyright file="MessageAdapter.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Application.Messages
{
    public class MessageAdapter : IMessageAdapter
    {
        private Dictionary<string, Type> _messages = new Dictionary<string, Type>();

        public void AddMessage(string key, Type value)
        {
            _messages.Add(key, value);
        }
      
        public IMessageConverter CreateConverter(string contentType, Encoding encoding, string protocol)
        {
            var converter = new MessageConverter(contentType: contentType, encoding: encoding, protocol: protocol);
            return converter;
        }

        public Type GetType(string name)
        {
            return _messages[name];
        }

        public string GetTypeName(Type type)
        {
            return type.Name;
        }
    }
}