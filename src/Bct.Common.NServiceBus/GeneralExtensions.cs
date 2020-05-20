// <copyright file="GeneralExtensions.cs" company="TerumoBCT">
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
   /// General extensions for NServiceBus
   /// </summary>
   public static class GeneralExtensions
   {
      /// <summary>
      /// Generic Extension function
      /// </summary>
      /// <typeparam name="T">The type of extension applied.</typeparam>
      /// <param name="t">Object applying extension to.</param>
      /// <param name="func">Action function to apply.</param>
      /// <returns>The Object applying extension to.</returns>
      public static T Do<T>(this T t, Action<T> func)
      {
         if (func == null)
         {
            throw new ArgumentNullException(nameof(func));
         }
         func(t);
         return t;
      }

      /// <summary>
      /// Adds a set of key-file sources to specified configuration builder.
      /// </summary>
      /// <param name="builder">The configuration builder applied to.</param>
      /// <param name="items">The list of directory paths with key-file configuration values..</param>
      /// <returns>The configuration builder returned for fluent operations.</returns>
      public static IConfigurationBuilder AddKeyPerFileSet(this IConfigurationBuilder builder, string[] items)
      {
         if (items != null)
         {
            foreach (var item in items)
            {
               builder.AddKeyPerFile(directoryPath: item, optional: true);
            }
         }
         return builder;
      }

   }
}