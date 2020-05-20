// <copyright file="PostgreSQLPersistenceStrategy.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Bct.Common.NServiceBus.Persistence
{
   internal class PostgreSQLPersistenceStrategy : IPersistenceStrategy
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

            var dialect = persistence.SqlDialect<SqlDialect.PostgreSql>();
            dialect.JsonBParameterModifier(
               modifier: parameter =>
               {
                  var npgsqlParameter = (NpgsqlParameter)parameter;
                  npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
               });

            persistence.ConnectionBuilder(
               connectionBuilder: () =>
               {
                  return new NpgsqlConnection(config.ConnectionString);
               });

            // When using Json.Net $type feature via TypeNameHandling, then MetadataPropertyHandling
            // should be set to ReadAhead.
            var settings = new JsonSerializerSettings
            {
               TypeNameHandling = TypeNameHandling.Auto,
               MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            };

            var sagaSettings = persistence.SagaSettings();
            sagaSettings.JsonSettings(settings);
         }
      }
   }
}