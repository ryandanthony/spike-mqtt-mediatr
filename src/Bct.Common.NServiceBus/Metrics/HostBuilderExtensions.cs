// <copyright file="HostBuilderExtensions.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace Bct.Common.NServiceBus.Metrics
{
   /// <summary>
   /// The HostBuilderExtensions class.
   /// </summary>
   public static class HostBuilderExtensions
   {
      /// <summary>
      /// Configure metrics.
      /// </summary>
      /// <param name="hostBuilder"> The hostBuilder.</param>
      /// <returns>The configured hostBuilder.</returns>
      public static IHostBuilder UseBctMetrics(this IHostBuilder hostBuilder)
      {
         if (hostBuilder == null)
         {
            throw new ArgumentNullException(nameof(hostBuilder));
         }

         return hostBuilder
             .ConfigureServices((ctx, services) =>
             {
                var configuration = ctx.Configuration;
                if (configuration["METRICS_ENABLED"] == "true")
                {
                   // Start the Metric Server to listen for scrape requests
                   var port = int.Parse(configuration["METRICS_PORT"], CultureInfo.InvariantCulture);
                   System.Console.WriteLine("PROMETHEUS PORT: {0}", port);

                   IMetricServer metricServerInstance = new MetricServer(port: port);
                   metricServerInstance.Start();
                }
             });
      }
   }
}