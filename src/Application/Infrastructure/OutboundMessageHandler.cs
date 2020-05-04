using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using MQTTnet.Extensions.ManagedClient;
using Spike.Common;

namespace Application.Infrastructure
{
    public class OutboundMessageHandler : IRequestHandler<MqttOutboundRequest, MqttOutboundResponse>
    {
        private readonly IManagedMqttClient _mqttClient;

        public OutboundMessageHandler(IManagedMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }

        public async Task<MqttOutboundResponse> Handle(MqttOutboundRequest request, CancellationToken cancellationToken)
        {
            await _mqttClient.PublishAsync(builder =>
                    builder
                        .WithTopic(request.Topic)
                        .WithUserProperty("messageType", request.GetMessageType())
                        .WithUserProperty("serialization", "json")
                        .WithUserProperty("encoding", "utf8")
                        .WithMessage(request.GetMessage())
                , cancellationToken);

            return new MqttOutboundResponse(){Success = true};
        }
    }
}
