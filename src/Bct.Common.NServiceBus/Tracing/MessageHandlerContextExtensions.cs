// <copyright file="MessageHandlerContextExtensions.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using NServiceBus;
using OpenTracing;

namespace Bct.Common.NServiceBus.Tracing
{
   /// <summary>
   /// The MessageHandlerContextExtensions class.
   /// </summary>
   public static class MessageHandlerContextExtensions
   {
      /// <summary>
      /// Get active span context using extension.
      /// </summary>
      /// <param name="context">The current context.</param>
      /// <returns>The span contract.</returns>
      public static ISpan GetSpan(this IMessageHandlerContext context)
      {
         if (context == null)
         {
            throw new ArgumentNullException(nameof(context));
         }

         return context.Extensions.Get<ISpan>();
      }
   }
}