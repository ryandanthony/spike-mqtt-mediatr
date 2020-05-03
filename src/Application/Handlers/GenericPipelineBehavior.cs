// <copyright file="GenericPipelineBehavior.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application.Handlers
{
    public class GenericPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            
            Console.WriteLine("-- Handling Request: " + request.GetType().FullName);
            var response = await next();
            Console.WriteLine("-- Finished Request: " + request.GetType().FullName);
            return response;
        }
    }
}