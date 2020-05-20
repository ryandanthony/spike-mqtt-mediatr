// <copyright file="MsSqlPersistenceStrategy.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Data.SqlClient;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Bct.Common.NServiceBus.Persistence
{
   internal class MsSqlPersistenceStrategy : IPersistenceStrategy
   {
      public void Configure(EndpointConfiguration endpointConfig, HostingConfiguration config)
      {

         if (!string.IsNullOrEmpty(config.ConnectionString))
         {
            var persistence = endpointConfig.UsePersistence<SqlPersistence>();
            if (config.SubscriptionPersisterCachingTimePeriodMinutes.HasValue)
            {
               var subscriptions = persistence.SubscriptionSettings();
               subscriptions.CacheFor(TimeSpan.FromMinutes(config.SubscriptionPersisterCachingTimePeriodMinutes.Value));
            }

            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(
               connectionBuilder: () =>
               {
                  return new SqlConnection(config.ConnectionString);
               });
         }
      }
   }
}