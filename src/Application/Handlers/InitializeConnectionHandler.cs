using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using Spike.Messages;

namespace Application.Handlers
{
    public class InitializeConnectionHandler : IRequestHandler<MqttInboundRequest<InitializeConnection>, MqttInboundResponse>
    {
        private readonly IMediator _mediator;

        public InitializeConnectionHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<MqttInboundResponse> Handle(MqttInboundRequest<InitializeConnection> request,
            CancellationToken cancellationToken)
        {
            var deviceType = "myDevice";
            Console.WriteLine($"[{request.Message.DeviceId}] InitializeConnection");

            var connectionAccepted = new ConnectionAccepted()
            {

            };
            Console.WriteLine($"Sending: ConnectionAccepted");
            await _mediator.Send(new MqttOutboundRequest()
            {
                Message = connectionAccepted,
                Topic = $"bct/{deviceType}/{request.Message.DeviceId}/in",
            }, cancellationToken);


            return new MqttInboundResponse() {Success = true};
        }
    }
}
