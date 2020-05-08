using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using Spike.Messages;
using Bct.Common.Workflow.Aggregates.Implementation;
using Microsoft.Extensions.Logging;
using BCT.Common.Logging.Extensions;
using System.Text;

namespace Application.Handlers
{
    public class InitializeConnectionHandler : IRequestHandler<InboundRequest<InitializeConnection>, OutboundNotification>
    {
        private readonly IMediator _mediator;
        private ILogger<InitializeConnectionHandler> _logger;
        private readonly IMessageAdapter _messageAdapter;

        public InitializeConnectionHandler(IMediator mediator, ILogger<InitializeConnectionHandler> logger, IMessageAdapter messageAdapter)
        {
            _mediator = mediator;
            _logger = logger;
            _messageAdapter = messageAdapter;
        }

        public async Task<OutboundNotification> Handle(InboundRequest<InitializeConnection> request,
            CancellationToken cancellationToken)
        {
            _logger.WithDebug("InitializeConnection: DeviceId:{0}", handling: Handling.Unrestricted, request.Message.DeviceId.Value)
                .WithPair("DeviceId", request.Message.DeviceId.Value)
                .Log();

            _logger.WithDebug("Sending: ConnectionAccepted").Log();

            // HANDLE REQUEST

            // In this case we respond now

            var responseAgg = new InitializeConnectionResponse();
            responseAgg.RequestApproved.Value = true;
            responseAgg.DeviceId.Value = request.Message.DeviceId.Value;

            var typeConverter = _messageAdapter.CreateConverter("TODO", Encoding.UTF8, "mqtt");
            var response = new OutboundNotification(typeConverter);
            response.Message = responseAgg;
            response.PayloadType = request.PropertyBag["responseType"] as string;
            response.Topic = request.ResponseTopic;
            response.CorrelationData = request.CorrelationData;

            await _mediator.Publish(response);

            return await Task.FromResult<OutboundNotification>(response);
        }
    }
}
