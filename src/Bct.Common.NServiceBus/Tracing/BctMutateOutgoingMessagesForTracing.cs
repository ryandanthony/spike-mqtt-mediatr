// <copyright file="BctMutateOutgoingMessagesForTracing.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.MessageMutator;

namespace Bct.Common.NServiceBus.Tracing
{
   /// <summary>
   /// The BctMutateOutgoingMessagesForTracing class.
   /// </summary>
   public class BctMutateOutgoingMessagesForTracing : IMutateOutgoingMessages
   {

      /// <summary>
      /// Mutate outgoing behavior.
      /// </summary>
      /// <param name="context"> The outgoing message context.</param>
      /// <returns>A task object.</returns>
      public Task MutateOutgoing(MutateOutgoingMessageContext context)
      {
         if (context == null)
         {
            throw new ArgumentNullException(nameof(context));
         }

         var traceId = string.Empty;

         // the outgoing headers
         var outgoingHeaders = context.OutgoingHeaders;
         if (context.TryGetIncomingHeaders(out var incomingHeaders))
         {
            // do something with the incoming headers
            var headers = (Dictionary<string, string>)incomingHeaders;
            if (!headers.TryGetValue(Constants.TracerKey, out traceId))
            {
               return Task.CompletedTask;
            }
         }

         // traceid will be blank when sending outgoing message for the first time
         if (!string.IsNullOrEmpty(traceId))
         {
            if (outgoingHeaders.All(p => p.Key != Constants.TracerKey))
            {
               outgoingHeaders.Add(Constants.TracerKey, traceId);
            }
         }

         return Task.CompletedTask;
      }
   }
}