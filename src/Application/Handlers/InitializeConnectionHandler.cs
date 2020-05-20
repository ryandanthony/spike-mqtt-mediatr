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
using Bct.Barcode;
using NServiceBus;
using Bct.Barcode.Contract.Queries;
using Bct.Barcode.Contract.Messages;

namespace Application.Handlers
{
    public class InitializeConnectionHandler : IRequestHandler<InboundRequest<InitializeConnection>, OutboundNotification>
    {
        private readonly IMediator _mediator;
        private ILogger<InitializeConnectionHandler> _logger;
        private readonly IMessageAdapter _messageAdapter;
        private readonly IMessageSession _ms;

        public InitializeConnectionHandler(IMediator mediator, ILogger<InitializeConnectionHandler> logger, IMessageAdapter messageAdapter, IMessageSession ms)
        {
            _mediator = mediator;
            _logger = logger;
            _messageAdapter = messageAdapter;
            _ms = ms;
        }

        public async Task<OutboundNotification> Handle(InboundRequest<InitializeConnection> request,
            CancellationToken cancellationToken)
        {
            _logger.WithDebug("InitializeConnection: DeviceId:{0}", handling: Handling.Unrestricted, request.Message.DeviceId.Value)
                .WithPair("DeviceId", request.Message.DeviceId.Value)
                .Log();

            _logger.WithDebug("Sending: ConnectionAccepted").Log();

            // HANDLE REQUEST
            var msg = new GetBarcode()
            {
                Id = 123
            };
            /// await _ms.Send(msg);

            // In this case we respond now

            var responseAgg = new InitializeConnectionResponse();
            ///responseAgg.DeviceId.Value = resp.Barcode.Name;  // *** NOTE - THIS IS NOT CORRECT JUST HERE FOR TESTING ***
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
