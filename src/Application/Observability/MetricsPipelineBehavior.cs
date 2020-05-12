// <copyright file="MetricsPipelineBehavior.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using Prometheus;

namespace Application.Observability
{
    public class MetricsPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private static readonly Histogram Duration = Metrics.CreateHistogram("mqtt_processing",
            "Histogram of time spent processing mqtt message.",
            new[] {"messageType"});

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            string label = null;
            Type t = request.GetType();
            if (request is IOutboundRequest)
            {
                label = (request as IOutboundRequest).MessageType;
            }
            else if (request is IInboundRequest)
            {
                label = (request as IInboundRequest).PropertyBag["messageType"] as string;
            }

            if (label != null)
            {
                string[] labels = { label };
                using (Duration.Labels(labels).NewTimer())
                {
                    var response = await next();
                    return response;
                }
            }
            else
            {
                return await next();
            }
        }
    }
}