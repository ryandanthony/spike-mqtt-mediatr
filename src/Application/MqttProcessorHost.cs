// <copyright file="MqttProcessorHost.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

// *** MQTT Component ***

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using Bct.Common.Workflow.Aggregates.Implementation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Spike.Messages;
using BCT.Common.Logging.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Bct.Common.Workflow.Aggregates;
using System.Text;
using System.Net.Mime;

namespace Application
{
    public class MqttProcessorHost : IHostedService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private IManagedMqttClient _mqttClient;
        private IMediator _mediator;
        private ILogger<MqttProcessorHost> _logger;
        private readonly IMessageAdapter _messageAdapter;

        public MqttProcessorHost(IServiceProvider provider, 
            ILogger<MqttProcessorHost> logger,
            IMessageAdapter messageAdapter,
            IEnumerable<IAbstractAggregate> abstractAggregates)
        {
            _provider = provider;
            _logger = logger;
            _messageAdapter = messageAdapter;

            foreach (var e in abstractAggregates)
            {
                _messageAdapter.AddMessage(e.GetType().Name, e.GetType());
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.WithInformation("Starting MqttProcessorHost").Log();
            _mqttClient = _provider.GetRequiredService<IManagedMqttClient>();
            _mediator = _provider.GetRequiredService<IMediator>();


            var group = "thisGroup";
            var deviceType = "+";
            var deviceId = "+";
            
            //var deviceType = "myDevice";
            //var deviceId = "myDevice-a";

            var topics = new[]
            {
                new TopicFilterBuilder().WithTopic( $"$share/{group}/bct/{deviceType}/{deviceId}/status").Build(),
                new TopicFilterBuilder().WithTopic( $"$share/{group}/bct/{deviceType}/{deviceId}/out").Build(),
            };
            await _mqttClient.SubscribeAsync(topics);
            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            _mqttClient.ApplicationMessageReceivedHandler =
                new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);
            _mqttClient.SynchronizingSubscriptionsFailedHandler =
                new SynchronizingSubscriptionsFailedHandlerDelegate(OnSynchronizingSubscriptionsFailed);
            _mqttClient.ApplicationMessageSkippedHandler =
                new ApplicationMessageSkippedHandlerDelegate(OnMessageSkipped);

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //BUT..... no? Unsubscribe instead?????
            await _mqttClient.StopAsync();
        }

        private Task OnSynchronizingSubscriptionsFailed(ManagedProcessFailedEventArgs e)
        {
            Console.WriteLine($"OnSynchronizingSubscriptionsFailed [Exception]:       {e.Exception}");
            return Task.CompletedTask;
        }

        private Task OnMessageSkipped(ApplicationMessageSkippedEventArgs e)
        {
            Console.WriteLine($"OnMessageSkipped");
            return Task.CompletedTask;
        }

        private Task OnConnected(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine($"OnConnected [AuthenticateResult]:       {e.AuthenticateResult}");
            return Task.CompletedTask;
        }

        private Task OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"OnDisconnected [ClientWasConnected]:       {e.ClientWasConnected}");
            Console.WriteLine($"OnDisconnected [AuthenticateResult]:       {e.AuthenticateResult}");
            Console.WriteLine($"OnDisconnected [Exception]:       {e.Exception}");
            return Task.CompletedTask;
        }

        private Task OnConnectingFailed(ManagedProcessFailedEventArgs e)
        {
            Console.WriteLine($"OnConnectingFailed [Exception]:       {e.Exception}");
            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            var messageTypeString = arg.ApplicationMessage
                .UserProperties?
                .FirstOrDefault(p => p.Name == "messageType")?
                .Value;

            var responseTypeString = arg.ApplicationMessage
                .UserProperties?
                .FirstOrDefault(p => p.Name == "responseType")?
                .Value;

            // Only if there is a responseType specifed do we do request/response processing
            // Otherwise we treat it as a notification.

            // Notification
            if (responseTypeString == null)
            {
                var messageType = MessageTypeMapper(messageTypeString);
                Type[] typeArgs = { messageType };
                var generic = typeof(InboundNotification<>);
                var constructedType = generic.MakeGenericType(typeArgs);
                var notifyObject = Activator.CreateInstance(constructedType);

                var inboundNotification = notifyObject as IInboundNotification;
                inboundNotification.RawMessage = arg.ApplicationMessage;
                inboundNotification.PropertyBag["messageType"] = messageTypeString;
                inboundNotification.TypeConverter = _messageAdapter.CreateConverter(arg.ApplicationMessage.ContentType, encoding: Encoding.UTF8, protocol: "mqtt");

                await _mediator.Publish(notifyObject);
            }
            // Request/Response
            else
            {
                var messageType = MessageTypeMapper(messageTypeString);
                Type[] typeArgs = { messageType };
                var generic = typeof(InboundRequest<>);
                var constructedType = generic.MakeGenericType(typeArgs);
                var requestObject = Activator.CreateInstance(constructedType);

                var inboundRequest = requestObject as IInboundRequest;
                inboundRequest.RawMessage = arg.ApplicationMessage;
                inboundRequest.PropertyBag["messageType"] = messageTypeString;
                inboundRequest.PropertyBag["responseType"] = responseTypeString;
                inboundRequest.ResponseTopic = arg.ApplicationMessage.ResponseTopic;
                inboundRequest.CorrelationData = arg.ApplicationMessage.CorrelationData;
                inboundRequest.TypeConverter = _messageAdapter.CreateConverter(arg.ApplicationMessage.ContentType, encoding: Encoding.UTF8, protocol: "mqtt");

                await _mediator.Send(requestObject);
             }
        }

        private Type MessageTypeMapper(string messageName)
        {
            try
            {
                return _messageAdapter.GetType(messageName);
            }
            catch (Exception)
            {
                throw new Exception("Unable to determine type");
            }
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
        }
    }

}