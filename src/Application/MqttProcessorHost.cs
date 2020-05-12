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

namespace Application
{
    public class MqttProcessorHost : IHostedService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private IManagedMqttClient _mqttClient;
        private IMediator _mediator;
        private ILogger<MqttProcessorHost> _logger;
        private IDictionary<string, Type> _registeredMessages = new Dictionary<string, Type>();
        private IEnumerable<IAbstractAggregate> _abstractAggregates;

        public MqttProcessorHost(IServiceProvider provider, ILogger<MqttProcessorHost> logger, IEnumerable<IAbstractAggregate> abstractAggregates)
        {
            _provider = provider;
            _logger = logger;
            _abstractAggregates = abstractAggregates;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.WithInformation("Starting MqttProcessorHost").Log();
            _mqttClient = _provider.GetRequiredService<IManagedMqttClient>();
            _mediator = _provider.GetRequiredService<IMediator>();

            // Build dictionary of aggregate names to types from all IAbstractAggregates to use in MessageTypeMapper
            foreach (var e in _abstractAggregates)
            {
                _registeredMessages.Add(e.GetType().Name, e.GetType());
            }

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

            // only if there is a responseType specifed do we do response processing
            if (responseTypeString == null)
            {
                var messageType = MessageTypeMapper(messageTypeString);
                Type[] typeArgs = { messageType };
                var generic = typeof(InboundRequest<>);
                var constructedType = generic.MakeGenericType(typeArgs);
                var requestObject = Activator.CreateInstance(constructedType);

                var inboundRequest = requestObject as IInboundRequest;
                inboundRequest.RawMessage = arg.ApplicationMessage;
                inboundRequest.PropertyBag["messageType"] = messageTypeString;

                var response = await _mediator.Send(requestObject) as InboundResponse;
                arg.ProcessingFailed = !response.Success;
            }
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

                var response = await _mediator.Send(requestObject) as InboundResponse;
                arg.ProcessingFailed = !response.Success;
                await Task.Run(async () => await _mqttClient.PublishAsync(builder =>
                        builder
                            .WithTopic(response.Topic)
                            .WithPayload(response.Payload)
                            .WithCorrelationData(response.CorrelationData)
                            .WithUserProperty("messageType", response.MessageType)
                            .WithUserProperty("payloadType", response.PayloadType)));
            }
        }

        private Type MessageTypeMapper(string messageName)
        {
            try
            {
                return _registeredMessages[messageName];
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