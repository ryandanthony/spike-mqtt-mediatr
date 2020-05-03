// <copyright file="MqttProcessorHost.cs" company="TerumoBCT">
// Copyright (c) 2020 TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using Spike.Messages;

namespace Application
{
    public class MqttProcessorHost : IHostedService, IDisposable
    {
        private readonly Assembly _handlerAssembly;
        private IMediator _mediator;
        private IManagedMqttClient _mqttClient;

        public MqttProcessorHost(Assembly handlerAssembly)
        {
            _handlerAssembly = handlerAssembly;
        }

        private void BuildMediator(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            var openTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestExceptionHandler<,,>),
                typeof(IRequestExceptionAction<,>),
                typeof(INotificationHandler<>),
                //typeof(IPipelineBehavior<,>),
            };

            foreach (var openType in openTypes)
            {
                builder
                    .RegisterAssemblyTypes(_handlerAssembly)
                    .AsClosedTypesOf(openType)
                    .AsImplementedInterfaces()
                    .AsSelf();
            }


            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.RegisterGeneric(typeof(MetricsPipelineBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            //Register additional services if needed

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var appName = "application";
            var clientId = $"{appName}-{Guid.NewGuid()}";
            Console.Title = $"Application: {clientId}";
            var options = new ManagedMqttClientOptionsBuilder()

                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(clientId)
                    .WithProtocolVersion(MqttProtocolVersion.V500)
                    .WithTcpServer("localhost", 1883)
                    //.WithTls()
                    .Build()
                )

                .Build();


            var builder = new ContainerBuilder();
            BuildMediator(builder);
            var container = builder.Build();
            _mediator = container.Resolve<IMediator>();

            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            var deviceType = "+";
            var deviceId = "+";
            //var deviceType = "myDevice";
            //var deviceId = "myDevice-a";
            var topic = $"$share/{appName}/bct/{deviceType}/{deviceId}/status";
            //var topic = $"bct/{deviceType}/{deviceId}/status";
            await _mqttClient.SubscribeAsync(new TopicFilterBuilder()
                .WithTopic(topic).Build());
            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            _mqttClient.ApplicationMessageReceivedHandler =
                new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);
            _mqttClient.SynchronizingSubscriptionsFailedHandler =
                new SynchronizingSubscriptionsFailedHandlerDelegate(OnSynchronizingSubscriptionsFailed);
            _mqttClient.ApplicationMessageSkippedHandler =
                new ApplicationMessageSkippedHandlerDelegate(OnMessageSkipped);

            await _mqttClient.StartAsync(options);
            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
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
            var messageTypeString = arg.ApplicationMessage.UserProperties?.FirstOrDefault(p => p.Name == "messageType")
                ?.Value;
            var messageType = MessageTypeMapper(messageTypeString);
            
            Type[] typeArgs = { messageType };
            var generic = typeof(GenericRequest<>);
            var constructed = generic.MakeGenericType(typeArgs);
            var genericRequest = Activator.CreateInstance(constructed);
            var theRequest = genericRequest as IGenericRequest;
            theRequest.RawMessage = arg.ApplicationMessage;
            theRequest.PropertyBag["messageType"] = messageTypeString;
            var response = await _mediator.Send(genericRequest) as GenericResponse;
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