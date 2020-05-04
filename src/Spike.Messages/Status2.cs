// <copyright file="Status2.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;

namespace Spike.Messages
{
    public class Status2
    {
        public string Value { get; set; }
        public DateTimeOffset When { get; set; }
        public Guid MessageId { get; set; }
        public string DeviceId { get; set; }
    }
}