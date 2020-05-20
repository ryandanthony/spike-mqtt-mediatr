// <copyright file="PromethusFeature.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using NServiceBus;
using NServiceBus.Features;

namespace Bct.Common.NServiceBus.Metrics
{
   internal class PrometheusFeature : Feature
   {
      private MetricsOptions _metricsOptions;

      private static string[] _labels =
      {
         "endpoint",
         "hostname",
         "endpointqueue",
         "category",
         "messageType",
      };

      private Dictionary<string, string> _nameMapping = new Dictionary<string, string>
      {
         // https://prometheus.io/docs/practices/naming/
         { "# of msgs successfully processed / sec", "nservicebus_success_total" },
         { "# of msgs pulled from the input queue /sec", "nservicebus_fetched_total" },
         { "# of msgs failures / sec", "nservicebus_failure_total" },
         { "Critical Time", "nservicebus_criticaltime_seconds" },
         { "Processing Time", "nservicebus_processingtime_seconds" },
         { "Retries", "nservicebus_retries_total" },
      };

      public PrometheusFeature()
      {
         Defaults(settings =>
         {
            _metricsOptions = settings.EnableMetrics();
         });
         //EnableByDefault();
      }

      protected override void Setup(FeatureConfigurationContext context)
      {
         var settings = context.Settings;
         var initialLabels = new[]
         {
            settings.EndpointName(),
            Dns.GetHostName(),
            settings.LocalAddress(),
         };

         _metricsOptions.RegisterObservers(
            register: probeContext => { RegisterProbes(probeContext, initialLabels); });
      }

      public void RegisterProbes(ProbeContext context, string[] initialLabels)
      {
         foreach (var duration in context.Durations)
         {
            if (!_nameMapping.ContainsKey(duration.Name))
            {
               continue;
            }

            var prometheusName = _nameMapping[duration.Name];
            var histogram = Prometheus.Metrics.CreateHistogram(prometheusName, duration.Description, _labels);
            duration.Register((ref DurationEvent @event) =>
            {
               var labelValues = GetMetricLabels(initialLabels, @event.MessageType);
               histogram.Labels(labelValues).Observe(@event.Duration.TotalSeconds);
            });
         }

         foreach (var signal in context.Signals)
         {
            if (!_nameMapping.ContainsKey(signal.Name))
            {
               continue;
            }
            var prometheusName = _nameMapping[signal.Name];
            var counter = Prometheus.Metrics.CreateCounter(prometheusName, signal.Description, _labels);
            signal.Register((ref SignalEvent @event) =>
            {
               var labelValues = GetMetricLabels(initialLabels, @event.MessageType);
               counter.Labels(labelValues).Inc();
            });
         }
      }

      private string[] GetMetricLabels(string[] initialLabels, string eventType)
      {
         var parts = eventType.Split(',');
         if (parts.Length > 0)
         {
            var msgType = parts[0];
            var msgCat = GetMessageCategory(msgType);
            var res = new List<string>(initialLabels) { msgCat, msgType };
            return res.ToArray();
         }
         return initialLabels;
      }

      private string GetMessageCategory(string type)
      {
         if (type == null)
         {
            throw new ArgumentNullException(nameof(type));
         }

         if (type.Contains("Contract.Command") || type.Contains("Contract.Commands"))
         {
            return "Command";
         }

         if (type.Contains("Contract.Event") || type.Contains("Contract.Events"))
         {
            return "Event";
         }

         if (type.Contains("Contract.Message") || type.Contains("Contract.Messages"))
         {
            return "Message";
         }

         if (type.Contains("Contract.Query") || type.Contains("Contract.Queries") || type.Contains("Contract.Express"))
         {
            return "Query";
         }

         return "Unknown";
      }
   }
}
