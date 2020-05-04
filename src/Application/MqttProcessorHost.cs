// <copyright file="MqttProcessorHost.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using Spike.Messages;

namespace Application
{
    public class MqttProcessorHost : IHostedService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private IManagedMqttClient _mqttClient;
        private IMediator _mediator;

        public MqttProcessorHost(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _mqttClient = _provider.GetRequiredService<IManagedMqttClient>();
            _mediator = _provider.GetRequiredService<IMediator>();

            string @group = "thisGroup";
            
            var deviceType = "+";
            var deviceId = "+";
            //var deviceType = "myDevice";
            //var deviceId = "myDevice-a";
            var topic = $"$share/{group}/bct/{deviceType}/{deviceId}/status";
            //var topic = $"bct/{deviceType}/{deviceId}/status";
            await _mqttClient.SubscribeAsync(topic, MqttQualityOfServiceLevel.ExactlyOnce);
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

            var messageType = MessageTypeMapper(messageTypeString);
            Type[] typeArgs = {messageType};
            var generic = typeof(MqttInboundRequest<>);
            var constructed = generic.MakeGenericType(typeArgs);
            var request = Activator.CreateInstance(constructed);
            var inboundRequest = request as IMqttInboundRequest;
            inboundRequest.RawMessage = arg.ApplicationMessage;
            inboundRequest.PropertyBag["messageType"] = messageTypeString;
            var response = await _mediator.Send(request) as MqttInboundResponse;
            arg.ProcessingFailed = !response.Success;
        }

        private static Type MessageTypeMapper(string value)
        {
            switch (value)
            {
                case nameof(Status):
                    return typeof(Status);
                case nameof(Status2):
                    return typeof(Status2);
                default:
                    throw new Exception("Unable to determine type");
            }
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
        }
    }
}