// <copyright file="MetricsHost.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Jaeger;
using Jaeger.Propagation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing.Propagation;
using OpenTracing.Util;
using Prometheus;

namespace Application
{
    public class DistributedTracingHost : IHostedService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private Tracer _tracer;

        public DistributedTracingHost(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var textMapCodec = new TextMapCodec.Builder().WithSpanContextKey("bct-tracing").Build();
            var jaegerConfiguration = Jaeger.Configuration.FromIConfiguration(_loggerFactory, _configuration);
            _tracer = jaegerConfiguration.GetTracerBuilder()
                .RegisterCodec(BuiltinFormats.TextMap, textMapCodec).Build();
            GlobalTracer.Register(_tracer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}