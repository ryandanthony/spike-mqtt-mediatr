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
    public class StatusHandler : IRequestHandler<InboundRequest<DeviceStatus>, InboundResponse>
    {
        private ILogger<StatusHandler> _logger;

        public StatusHandler(ILogger<StatusHandler> logger)
        {
            _logger = logger;
        }

        public Task<InboundResponse> Handle(InboundRequest<DeviceStatus> request, CancellationToken cancellationToken)
        {
            _logger.WithDebug("DeviceStatus: DeviceId:{0} Condition:{1}", request.Message.DeviceId.Value, request.Message.Condition.Value)
                .HandleAs(Handling.Unrestricted)
                .WithPair("DeviceId", request.Message.DeviceId.Value)
                .WithPair("MessageId",request.Message.MessageId.Value)
                .WithPair("When", DateTime.FromBinary(Convert.ToInt64(request.Message.When.Value)).ToString())
                .WithPair("Condition", request.Message.Condition.Value)
                .Log();
            return Task.FromResult(new InboundResponse() {Success = true});
        }
    }
}
