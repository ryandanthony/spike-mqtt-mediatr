// <copyright file="MetricsHost.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace Application
{
    public class MetricsHost : IHostedService
    {
        private readonly MetricServer _metricServerInstance;

        public MetricsHost(int port)
        {
            _metricServerInstance = new MetricServer(port: port);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _metricServerInstance.Start();
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _metricServerInstance.StopAsync();
        }
    }
}