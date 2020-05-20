// <copyright file="HostBuilderExtensions.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using Jaeger.Propagation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Util;

namespace Bct.Common.NServiceBus.Tracing
{
   /// <summary>
   /// The HostBuilderExtensions class.
   /// </summary>
   public static class HostBuilderExtensions
   {
      /// <summary>
      /// Configure tracing.
      /// </summary>
      /// <param name="hostBuilder"> The hostBuilder.</param>
      /// <returns>The configured hostBuilder.</returns>
      public static IHostBuilder UseJaegerTracing(this IHostBuilder hostBuilder)
      {
         if (hostBuilder == null)
         {
            throw new ArgumentNullException(nameof(hostBuilder));
         }

         return hostBuilder
            .ConfigureHostConfiguration(configHost =>
            {
               var initialData = new Dictionary<string, string>
               {
                  { "JAEGER_SERVICE_NAME", Assembly.GetEntryAssembly()?.GetName().Name },
               };
               configHost.AddInMemoryCollection(initialData);
            })
            .ConfigureServices((context, services) =>
            {
               var configuration = context.Configuration;

               //Configuration can be found at: https://github.com/jaegertracing/jaeger-client-csharp#configuration-via-environment
               //We are using IConfiguration
               if (configuration["JAEGER_ENABLED"] == "true")
               {
                  services.AddSingleton<ITracer>(serviceProvider =>
                  {
                     var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                     var textMapCodec = new TextMapCodec.Builder().WithSpanContextKey(Constants.TracerKey).Build();
                     var jaegerConfiguration = Jaeger.Configuration.FromIConfiguration(loggerFactory, configuration);
                     var tracer = jaegerConfiguration.GetTracerBuilder()
                        .RegisterCodec(BuiltinFormats.TextMap, textMapCodec).Build();
                     GlobalTracer.Register(tracer);
                     return tracer;
                  });
               }
               else
               {
                  services.AddSingleton<ITracer>(serviceProvider =>
                     GlobalTracer.Instance); //Use the NoopTracer built in.
               }
            });
      }
   }
}