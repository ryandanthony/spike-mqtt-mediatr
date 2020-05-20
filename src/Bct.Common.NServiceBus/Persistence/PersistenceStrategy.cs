// <copyright file="PersistenceStrategy.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using NServiceBus;

namespace Bct.Common.NServiceBus.Persistence
{
   internal interface IPersistenceStrategy
   {
      void Configure(EndpointConfiguration endpointConfig, HostingConfiguration config);
   }
}
