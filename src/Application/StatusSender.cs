// <copyright file="StatusSender.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spike.Messages;
using BCT.Common.Logging.Extensions;

namespace Application
{
    internal class StatusSender : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<StatusSender> _logger;

        public StatusSender(IServiceProvider provider, ILogger<StatusSender> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                var mediator = _provider.GetRequiredService<IMediator>();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var status = new AppStatus()
                    {
                        Value = "App 1",
                        When = DateTimeOffset.Now,
                        MessageId = Guid.NewGuid(),
                    };
                    _logger.WithDebug($"App 1 Sending Status: {status.MessageId}").Log();
                    await mediator.Publish(new OutboundNotification()
                    {
                        Topic = $"bct/app/status",
                        Message = status,
                        MessageType = status.GetType().Name
                    }, cancellationToken);
                    await Task.Delay(10000, cancellationToken);
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}