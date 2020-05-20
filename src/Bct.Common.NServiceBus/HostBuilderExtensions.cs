// <copyright file="HostBuilderExtensions.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using OpenTracing;

namespace Bct.Common.NServiceBus
{
   /// <summary>
   /// The HostBuilderExtensions class.
   /// </summary>
   public static class HostBuilderExtensions
   {
      /// <summary>
      /// Configure NServiceBus.
      /// </summary>
      /// <param name="hostBuilder"> The hostBuilder.</param>
      /// <param name="configuration"> The configuration.</param>
      /// <param name="configureEndpoint"> The configuration usd to create the endpoint instance.</param>
      /// <returns>The configured hostBuilder.</returns>
      public static IHostBuilder UseBctNServiceBus(
         this IHostBuilder hostBuilder,
         Func<IConfiguration, HostingConfiguration> configuration,
         Action<EndpointConfiguration> configureEndpoint = null)
      {
         if (hostBuilder == null)
         {
            throw new ArgumentNullException(nameof(hostBuilder));
         }

         hostBuilder.ConfigureServices((ctx, serviceCollection) =>
         {
            var config = configuration(ctx.Configuration);
            var endpointConfiguration = EndpointSetup.Create(config);
            configureEndpoint?.Invoke(endpointConfiguration);
            var startableEndpoint = EndpointWithExternallyManagedContainer.Create(
               endpointConfiguration,
               new ServiceCollectionAdapter(serviceCollection));

            serviceCollection.AddSingleton(serviceProvider => startableEndpoint.MessageSession.Value);
            serviceCollection.AddSingleton<IHostedService>(serviceProvider =>
            {
               //This code will ensure that the ITracer is available for the pipeline. DO NOT DELETE!
               var tracer = serviceProvider.GetService<ITracer>();
               if (tracer == null)
               {
                  throw new Exception("ITracer not found.");
               }

               /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

               return new NServiceBusHostedService(startableEndpoint, serviceProvider);
            });

         });

         return hostBuilder;
      }
   }
}