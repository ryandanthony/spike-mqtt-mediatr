// <copyright file="ConventionExtensions.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using NServiceBus;

namespace Bct.Common.NServiceBus
{
   /// <summary>
   /// The ConventionExtensions class.
   /// </summary>
   public static class ConventionExtensions
   {
      /// <summary>
      /// Add Endpoint Conventions.
      /// </summary>
      /// <param name="config"> The endpoint configuration.</param>
      public static void AddBctConventions(this EndpointConfiguration config)
      {
         if (config == null)
         {
            throw new ArgumentNullException(nameof(config));
         }

         var conventions = config.Conventions();
         conventions.DefiningCommandsAs(type =>
            type.AssemblyQualifiedName != null && (type.AssemblyQualifiedName.Contains("Contract.Command") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Commands")));

         conventions.DefiningEventsAs(type =>
            type.AssemblyQualifiedName != null && (type.AssemblyQualifiedName.Contains("Contract.Event") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Events")));

         conventions.DefiningMessagesAs(type =>
            type.AssemblyQualifiedName != null && (type.AssemblyQualifiedName.Contains("Contract.Message") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Messages") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Query") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Queries") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Express")));

         conventions.DefiningExpressMessagesAs(type =>
            type.AssemblyQualifiedName != null && (type.AssemblyQualifiedName.Contains("Contract.Query") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Queries") ||
                                                    type.AssemblyQualifiedName.Contains("Contract.Express")));

         conventions.DefiningTimeToBeReceivedAs(
            type =>
            {
               if (type.AssemblyQualifiedName != null && (type.AssemblyQualifiedName.Contains("Contract.Query") ||
                                                           type.AssemblyQualifiedName.Contains("Contract.Queries")))
               {
                  return TimeSpan.FromSeconds(60);
               }

               return TimeSpan.MaxValue;
            });

         conventions.DefiningDataBusPropertiesAs(property => property.Name.EndsWith("FileBytes", StringComparison.InvariantCulture));
      }
   }
}