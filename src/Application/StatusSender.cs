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
using Spike.Messages;

namespace Application
{
    internal class StatusSender : IHostedService
    {
        private readonly IServiceProvider _provider;

        public StatusSender(IServiceProvider provider)
        {
            _provider = provider;
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
                    Console.WriteLine($"Sending: {status.MessageId}");
                    await mediator.Send(new MqttOutboundRequest()
                    {
                        Topic = $"bct/app/status",
                        Message = status,
                        MessageType = status.GetType().Name
                    }, cancellationToken);
                    await Task.Delay(1000, cancellationToken);
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}