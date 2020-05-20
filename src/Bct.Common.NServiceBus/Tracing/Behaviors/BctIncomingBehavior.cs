// <copyright file="BctIncomingBehavior.cs" company="TerumoBCT">
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
   /// The BctIncomingBehavior class.
   /// </summary>
   public class BctIncomingBehavior : Behavior<IIncomingLogicalMessageContext>
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BctIncomingBehavior"/> class.
      /// </summary>
      public BctIncomingBehavior()
      {
      }

      /// <summary>
      /// Invoke handler for incoming logical message pipeline stage.
      /// </summary>
      /// <param name="context">The current context.</param>
      /// <param name="next">The next Behavior in the chain to execute.</param>
      /// <returns>Returns a task object.</returns>
      public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
      {
         if (context == null)
         {
            throw new ArgumentNullException(nameof(context));
         }

         if (next == null)
         {
            throw new ArgumentNullException(nameof(next));
         }

         var operationName = $"Handle: {context.Message.MessageType.FullName}";
         using (var scope = GlobalTracer.Instance.StartServerSpan(operationName, context))
         {
            context.Extensions.Set<ISpan>(scope.Span);
            await next().ConfigureAwait(true);
         }
      }
   }
}