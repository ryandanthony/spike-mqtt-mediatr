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
            if (request.Payload != null)
            {
                await _mqttClient.PublishAsync(builder =>
                        builder
                            .WithTopic(request.Topic)
                            .WithMessage(request.Message)
                            .WithPayload(request.Payload)
                            .WithCorrelationData(request.CorrelationData)
                            .WithUserProperty("messageType", request.MessageType)
                            .WithUserProperty("payloadType", request.PayloadType)
                            .WithUserProperty("serialization", "json")
                            .WithUserProperty("encoding", "utf8")
                    , cancellationToken);
            }
            else
            {
                await _mqttClient.PublishAsync(builder =>
                        builder
                            .WithTopic(request.Topic)
                            .WithMessage(request.Message)
                            .WithUserProperty("messageType", request.MessageType)
                    , cancellationToken);
            }

            return new MqttOutboundResponse(){Success = true};
        }
    }
}
