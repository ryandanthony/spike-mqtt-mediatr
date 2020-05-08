using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using Bct.Common.Workflow.Aggregates.Implementation;
using BCT.Common.Logging.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Spike.Messages;

namespace Application.Handlers
{
    public class StatusHandler2 : INotificationHandler<InboundNotification<DeviceStatus>>
    {
        private ILogger<StatusHandler> _logger;

        public StatusHandler2(ILogger<StatusHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(InboundNotification<DeviceStatus> notification, CancellationToken cancellationToken)
        {
            _logger.WithDebug("DeviceStatus-Secondhandler: DeviceId:{0} Condition:{1}", notification.Message.DeviceId.Value, notification.Message.Condition.Value)
                .HandleAs(Handling.Unrestricted)
                .WithPair("DeviceId", notification.Message.DeviceId.Value)
                .WithPair("MessageId", notification.Message.MessageId.Value)
                .WithPair("When", DateTime.FromBinary(Convert.ToInt64(notification.Message.When.Value)).ToString())
                .WithPair("Condition", notification.Message.Condition.Value)
                .Log();
            return Task.CompletedTask;
        }

    }
}
