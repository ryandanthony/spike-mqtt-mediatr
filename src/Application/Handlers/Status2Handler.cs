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
    public class Status2Handler : IRequestHandler<MqttInboundRequest<Status2>, MqttInboundResponse>
    {
        public Task<MqttInboundResponse> Handle(MqttInboundRequest<Status2> request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"2[{request.Message.DeviceId}][{request.Message.MessageId}][{request.Message.When}] {request.Message.Value}" );
            return Task.FromResult(new MqttInboundResponse() {Success = true});
        }
    }
}
