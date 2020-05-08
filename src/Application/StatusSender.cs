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
using System.Text;
using Bct.Common.Workflow.Aggregates.Implementation;

namespace Application
{
    internal class StatusSender : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<StatusSender> _logger;
        private readonly IMessageAdapter _messageAdapter;

        public StatusSender(IServiceProvider provider, ILogger<StatusSender> logger, IMessageAdapter messageAdapter)
        {
            _provider = provider;
            _logger = logger;
            _messageAdapter = messageAdapter;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                var mediator = _provider.GetRequiredService<IMediator>();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var status = new AppStatus();
                    status.Name.Value = "App 1";
                    status.MessageId.Value = Guid.NewGuid().ToString();
                    status.When.Value = "TODO TESTING";

                    var typeConverter = _messageAdapter.CreateConverter("TODO", Encoding.UTF8, "mqtt");
                    var outbound = new OutboundNotification(typeConverter);
                    outbound.Topic = $"bct/app/status";
                    outbound.Message = status;
                    outbound.PayloadType = status.GetType().Name;

                   _logger.WithDebug($"App 1 Sending Status: {status.MessageId}").Log();

                    await mediator.Publish(outbound, cancellationToken);
                    await Task.Delay(20000, cancellationToken);
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}