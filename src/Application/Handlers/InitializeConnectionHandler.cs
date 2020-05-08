using System;
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
    public class InitializeConnectionHandler : IRequestHandler<MqttInboundRequest<InitializeConnection>, MqttInboundResponse>
    {
        private readonly IMediator _mediator;
        private ILogger<InitializeConnectionHandler> _logger;
        public InitializeConnectionHandler(IMediator mediator, ILogger<InitializeConnectionHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<MqttInboundResponse> Handle(MqttInboundRequest<InitializeConnection> request,
            CancellationToken cancellationToken)
        {
            _logger.WithDebug("InitializeConnection: DeviceId:{0}", handling: Handling.Unrestricted, request.Message.DeviceId.Value)
                .WithPair("DeviceId", request.Message.DeviceId.Value)
                .Log();

            var connectionAccepted = new ConnectionAccepted()
            {

            };

            _logger.WithDebug("Sending: ConnectionAccepted").Log();

            var response = new InitializeConnectionResponse();
            response.RequestApproved.Value = true;
            response.DeviceId.Value = request.Message.DeviceId.Value;
            var payload = Encoding.UTF8.GetBytes(response.Serialize());

            return new MqttInboundResponse()
            {
                    Topic = request.ResponseTopic,
                    Message = connectionAccepted,
                    MessageType = connectionAccepted.GetType().Name,
                    Payload = payload,
                    PayloadType = response.GetType().Name,
                    CorrelationData = request.CorrelationData,
            };
        }
    }
}
