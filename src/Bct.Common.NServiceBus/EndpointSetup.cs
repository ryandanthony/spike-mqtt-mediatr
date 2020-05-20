// <copyright file="EndpointSetup.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Net;

using Bct.Common.NServiceBus.Logging;
using Bct.Common.NServiceBus.Metrics;
using Bct.Common.NServiceBus.Persistence;
using Bct.Common.NServiceBus.Tracing;
using Bct.Common.NServiceBus.Tracing.Behaviors;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.MessageMutator;

namespace Bct.Common.NServiceBus
{
   public static class EndpointSetup
   {
      public static EndpointConfiguration Create(HostingConfiguration configuration)
      {
         var loggerDefinition = LogManager.Use<BctLoggerDefinition>();
         loggerDefinition.Level(configuration.LoggingMinLevel);

         var endpointConfiguration = new EndpointConfiguration(configuration.EndpointName);

         endpointConfiguration.EnableInstallers();
         endpointConfiguration.ConfigureUniqueAddress(configuration);
         endpointConfiguration.AddBctConventions();
         endpointConfiguration.EnableCallbacks();
         endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

         var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
         transport.UseConventionalRoutingTopology();
         var routing = transport.Routing();
         foreach (var route in configuration.EndpointRoutes)
         {
            routing.RouteToEndpoint(route.Key, route.Value);
         }

         if (configuration.EnablePersistence || configuration.EnableOutbox)
         {
            // Persistence must be configured to enable outbox.
            ConfigurePersistence(configuration, endpointConfiguration);
         }

         if (configuration.EnableOutbox)
         {
            // Enable the Outbox. Duplicates will be suppressed.
            endpointConfiguration.EnableOutbox();
         }

         // Limit maximum concurrency so that no more messages than the specified value
         // are ever being processed at the same time. Set this value to 1 to process
         // messages sequentially.
         if (configuration.LimitMessageProcessingConcurrencyNumProcessors.HasValue)
         {
            endpointConfiguration.LimitMessageProcessingConcurrencyTo(configuration
               .LimitMessageProcessingConcurrencyNumProcessors.Value);
         }

         transport.ConnectionString(configuration.RabbitMqConnectionString);

         if (configuration.SendOnly)
         {
            endpointConfiguration.SendOnly();
         }

         if (configuration.EnableMetrics)
         {
            endpointConfiguration.EnableFeature<PrometheusFeature>();
         }

         //Setup Tracing
         endpointConfiguration.RegisterMessageMutator(new BctMutateOutgoingMessagesForTracing());
         var pipeline = endpointConfiguration.Pipeline;
         pipeline.Register(
            behavior: new BctOutgoingBehavior(),
            description: "Added for Jaeger tracing id to be available in outgoing context message header");
         pipeline.Register(
            behavior: new BctIncomingBehavior(),
            description: "Added for Jaeger tracing id to be available in incoming context message header");

         return endpointConfiguration;
      }

      private static void ConfigurePersistence(HostingConfiguration configuration, EndpointConfiguration endpointConfig)
      {
         if (!string.IsNullOrEmpty(configuration.ConnectionString))
         {
            switch (configuration.PersistenceType)
            {
               case "MSSQL":
                  new MsSqlPersistenceStrategy().Configure(endpointConfig, configuration);
                  break;
               default:
                  new PostgreSQLPersistenceStrategy().Configure(endpointConfig, configuration);
                  break;
            }
         }
      }

      private static void ConfigureUniqueAddress(this EndpointConfiguration endpointConfig, HostingConfiguration config)
      {
         switch (config.UniqueAddressType)
         {
            case UniqueAddressType.HostnameBased:
               endpointConfig.MakeInstanceUniquelyAddressable(Dns.GetHostName());
               break;
            case UniqueAddressType.RandomBased:
               endpointConfig.MakeInstanceUniquelyAddressable(Guid.NewGuid().ToString());
               break;
            case UniqueAddressType.UserSpecified:
               endpointConfig.MakeInstanceUniquelyAddressable(config.UniqueAddressValue);
               break;
            default:
               endpointConfig.MakeInstanceUniquelyAddressable(Dns.GetHostName());
               break;
         }
      }
   }
}
