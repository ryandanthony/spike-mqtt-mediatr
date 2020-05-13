using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using MQTTnet.Extensions.ManagedClient;
using Spike.Common;

namespace Application.Infrastructure
{
    public class OutboundRequestHandler : IRequestHandler<OutboundRequest, OutboundResponse>
    {
        private readonly IManagedMqttClient _mqttClient;

        public OutboundRequestHandler(IManagedMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }

        public async Task<OutboundResponse> Handle(OutboundRequest request, CancellationToken cancellationToken)
        {

            // TODO handle response

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

            return new OutboundResponse(){Success = true};
        }
    }
}
