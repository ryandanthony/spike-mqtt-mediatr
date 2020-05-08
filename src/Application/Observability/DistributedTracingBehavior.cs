// <copyright file="MetricsPipelineBehavior.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;

namespace Application.Observability
{
    public class DistributedTracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        //private static readonly Histogram Duration = Metrics.CreateHistogram("mqtt_processing",
        //    "Histogram of time spent processing mqtt message.",
        //    new[] {"messageType"});

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            //var labels = new[]
            //{
            //    request.PropertyBag["messageType"] as string
            //};
            //using (Duration.Labels(labels).NewTimer())
            //{
                var response = await next();
                return response;
            //}
        }
    }
}