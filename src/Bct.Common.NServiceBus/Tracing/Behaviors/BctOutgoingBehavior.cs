// <copyright file="BctOutgoingBehavior.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using OpenTracing;
using OpenTracing.Util;

namespace Bct.Common.NServiceBus.Tracing.Behaviors
{
   /// <summary>
   /// The BctOutgoingBehavior class.
   /// </summary>
   public class BctOutgoingBehavior : Behavior<IOutgoingLogicalMessageContext>
   {
      /// <summary>
      /// Invoke handler for outgoing logical message pipeline stage.
      /// </summary>
      /// <param name="context">The current context.</param>
      /// <param name="next">The next Behavior in the chain to execute.</param>
      /// <returns>The task object.</returns>
      public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
      {
         if (context == null)
         {
            throw new ArgumentNullException(nameof(context));
         }

         if (next == null)
         {
            throw new ArgumentNullException(nameof(context));
         }

         var operationName = $"Outbound: {context.Message.MessageType.FullName}";
         using (var scope = GlobalTracer.Instance.StartServerSpan(operationName, context))
         {
            await next().ConfigureAwait(true);
         }
      }
   }
}