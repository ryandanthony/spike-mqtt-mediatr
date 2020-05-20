// <copyright file="HostingConfiguration.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Reflection;

namespace Bct.Common.NServiceBus
{
   /// <summary>
   /// The HostingConfiguration class.
   /// </summary>
   public class HostingConfiguration
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="HostingConfiguration"/> class.
      /// </summary>
      public HostingConfiguration()
      {
         EndpointRoutes = new Dictionary<Assembly, string>();
      }

      /// <summary>
      /// Gets or sets the EndpointName.
      /// </summary>
      /// <value>
      /// The value of the EndpointName.
      /// </value>
      public string EndpointName { get; set; }

      /// <summary>
      /// Gets or sets the RabbitMq Connection String.
      /// </summary>
      /// <value>
      /// The value of the Connection String.
      /// </value>
      public string RabbitMqConnectionString { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether to SendOnly.
      /// </summary>
      /// <value>
      /// The boolean SendOnly value.
      /// </value>
      public bool SendOnly { get; set; }

      /// <summary>
      /// Gets the EndpointRoutes.
      /// </summary>
      /// <value>
      /// The EndpointRoutes.
      /// </value>
      public Dictionary<Assembly, string> EndpointRoutes { get; private set; }

      /// <summary>
      /// Gets or sets the LoggingMinLevel.
      /// </summary>
      /// <value>
      /// The LoggingMinLevel string.
      /// </value>
      public string LoggingMinLevel { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether to EnablePersistence.
      /// </summary>
      /// <value>
      /// The EnablePersistence.
      /// </value>
      public bool EnablePersistence { get; set; }

      /// <summary>
      /// Gets or sets a value indicating the PersistenceType.
      /// </summary>
      /// <value>
      /// The boolean value.
      /// </value>
      public string PersistenceType { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether to EnableOutbox.
      /// </summary>
      /// <value>
      /// The boolean value.
      /// </value>
      public bool EnableOutbox { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether to EnableMetrics
      /// </summary>
      public bool EnableMetrics { get; set; }

      /// <summary>
      /// Gets or sets the ConnectionString.
      /// </summary>
      /// <value>
      /// The ConnectionString.
      /// </value>
      public string ConnectionString { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether to LimitMessageProcessingConcurrencyNumProcessors.
      /// </summary>
      /// <value>
      /// The LimitMessageProcessingConcurrencyNumProcessors int value.
      /// </value>
      public int? LimitMessageProcessingConcurrencyNumProcessors { get; set; }

      /// <summary>
      /// Gets or sets a value indicating the UniqueAddressType.
      /// </summary>
      /// <value>
      /// The UniqueAddressType.
      /// </value>
      public UniqueAddressType UniqueAddressType { get; set; }

      /// <summary>
      /// Gets or sets a value indicating the UniqueAddressValue.
      /// </summary>
      /// <value>
      /// The UniqueAddressValue.
      /// </value>
      public string UniqueAddressValue { get; set; }

      /// <summary>
      /// Gets or sets the Subscription Persister's Caching Time Period in Minutes.
      /// </summary>
      /// <value>
      /// The value in minutes.
      /// </value>
      public int? SubscriptionPersisterCachingTimePeriodMinutes { get; set; }
   }
}
