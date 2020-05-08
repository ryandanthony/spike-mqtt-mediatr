// <copyright file="MessageConverter.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using Bct.Common.Workflow.Aggregates;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;


namespace Application.Messages
{
    public class MessageConverter : IMessageConverter
    {
        private string _contentType;
        private Encoding _encoding;
        private string _protocol;

        public MessageConverter(string contentType, Encoding encoding, string protocol)
        {
            _contentType = contentType;
            _encoding = encoding;
            _protocol = protocol;
        }
        public T ConvertToObject<T>(byte[] raw) where T: class
        {
            string ret1;
            if (Encoding.UTF8 == _encoding)
            {
                ret1 = Encoding.UTF8.GetString(raw);
            }
            else
            {
                throw new Exception($"Invalid encoding: {_encoding}");
            }
            
            T ret = BaseAggregate.Deserialize<T>(ret1);
            return ret;
        }

        public byte[] ConvertToBytes<T>(T obj) where T: class
        {
            var ret1 = BaseAggregate.Serialize(obj as BaseAggregate);
            byte[] ret;
            if (Encoding.UTF8 == _encoding)
            {
                ret = Encoding.UTF8.GetBytes(ret1);
            }
            else
            {
                throw new Exception($"Invalid encoding: {_encoding}");
            }
            return ret;
        }
    }

}