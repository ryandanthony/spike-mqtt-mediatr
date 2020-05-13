using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using MQTTnet.Extensions.ManagedClient;
using Spike.Common;

namespace Application.Infrastructure
{
    public class OutboundNotificationHandler : INotificationHandler<OutboundNotification>
    {
        private readonly IManagedMqttClient _mqttClient;

        public OutboundNotificationHandler(IManagedMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }

        public async Task Handle(OutboundNotification notification, CancellationToken cancellationToken)
        {
            if (notification.Payload != null)
            {
                await _mqttClient.PublishAsync(builder =>
                        builder
                            .WithTopic(notification.Topic)
                            .WithMessage(notification.Message)
                            .WithPayload(notification.Payload)
                            .WithUserProperty("messageType", notification.MessageType)
                            .WithUserProperty("payloadType", notification.PayloadType)
                            .WithUserProperty("serialization", "json")
                            .WithUserProperty("encoding", "utf8")
                    , cancellationToken);
            }
            else
            {
                await _mqttClient.PublishAsync(builder =>
                        builder
                            .WithTopic(notification.Topic)
                            .WithMessage(notification.Message)
                            .WithUserProperty("messageType", notification.MessageType)
                    , cancellationToken);
            }
        }


    }
}
