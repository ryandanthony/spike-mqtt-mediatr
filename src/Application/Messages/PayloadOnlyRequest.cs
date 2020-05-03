// <copyright file="PayloadOnlyRequest.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using MediatR;

namespace Application.Messages
{
    public class PayloadOnlyRequest : IRequest<PayloadOnlyResponse>
    {
    }
}