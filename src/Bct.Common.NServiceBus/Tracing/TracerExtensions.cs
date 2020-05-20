// <copyright file="TracerExtensions.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus.Pipeline;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

namespace Bct.Common.NServiceBus.Tracing
{
   internal static class TracerExtensions
   {
      /// <summary>
      /// Extract Jaeger trace id to build child span under same trace.
      /// </summary>
      /// <returns>The scope.</returns>
      public static IScope StartServerSpan(
         this ITracer tracer,
         string operation,
         IIncomingLogicalMessageContext context)
      {
         return ExtractAndBuild(tracer, operation, context.Headers);
      }

      /// <summary>
      /// Extract Jaeger trace id to build child span under same trace.
      /// </summary>
      /// <returns>The scope.</returns>
      public static IScope StartServerSpan(
         this ITracer tracer,
         string operation,
         IOutgoingLogicalMessageContext context)
      {
         if (context.Headers.ContainsKey(Constants.TracerKey))
         {
            return ExtractAndBuild(tracer, operation, context.Headers);
         }
         else
         {
            return BuildAndInject(tracer, operation, context.Headers);
         }
      }

      /// <summary>
      /// Extract headers and extend parent span context object.
      /// </summary>
      /// <returns>The scope.</returns>
      private static IScope ExtractAndBuild(ITracer tracer, string spanName, IDictionary<string, string> headers)
      {
         //Extracting parent span to get the parent Span context
         var parentSpanCtx = tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(headers));

         var spanBuilder = BuildSpanWithTag(tracer, spanName);
         if (parentSpanCtx != null)
         {
            spanBuilder = spanBuilder.AsChildOf(parentSpanCtx);
         }

         var scope = StartActiveSpan(spanBuilder);
         return scope;
      }

      /// <summary>
      /// Build new span context object and inject trace id into headers.
      /// </summary>
      /// <param name="tracer">The tracer.</param>
      /// <param name="spanName">The spanName.</param>
      /// <param name="headers">The headers.</param>
      /// <returns>The scope.</returns>
      private static IScope BuildAndInject(ITracer tracer, string spanName, IDictionary<string, string> headers)
      {
         var dictionary = new Dictionary<string, string>();

         //Build span with tag
         var spanBuilder = BuildSpanWithTag(tracer, spanName);

         //mark that span as active
         var scope = StartActiveSpan(spanBuilder);

         //Injection done to inject tracer id into dictionary
         tracer.Inject(scope.Span.Context, BuiltinFormats.TextMap, new TextMapInjectAdapter(dictionary));

         //appending headers with trace id
         foreach (var dic in dictionary)
         {
            if (!headers.ContainsKey(dic.Key))
            {
               headers.Add(dic.Key, dic.Value);
            }
         }

         return scope;
      }

      /// <summary>
      /// Takes tracer object and provide a span context.
      /// </summary>
      /// <param name="tracer">The tracer.</param>
      /// <param name="spanName">The spanName.</param>
      /// <returns>The spanbuilder.</returns>
      private static ISpanBuilder BuildSpanWithTag(ITracer tracer, string spanName)
      {
         return tracer.BuildSpan(spanName).WithTag(Tags.SpanKind, Tags.SpanKindConsumer);
      }

      /// <summary>
      /// Takes the span context and start active span.
      /// </summary>
      /// <param name="span">The span.</param>
      /// <returns>The scope.</returns>
      private static IScope StartActiveSpan(ISpanBuilder span)
      {
         return span.StartActive(true);
      }
   }
}
