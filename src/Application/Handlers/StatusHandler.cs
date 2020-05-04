using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using Spike.Messages;

namespace Application.Handlers
{
    public class StatusHandler : IRequestHandler<MqttInboundRequest<Status>, MqttInboundResponse>
    {
        public Task<MqttInboundResponse> Handle(MqttInboundRequest<Status> request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{request.Message.DeviceId}][{request.Message.MessageId}][{request.Message.When}] {request.Message.Value}" );
            return Task.FromResult(new MqttInboundResponse() {Success = true});
        }
    }
}
