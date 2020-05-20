// <copyright file="NServiceBusHostedService.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace Bct.Common.NServiceBus
{
   internal sealed class NServiceBusHostedService : IHostedService, IDisposable
   {
      private IEndpointInstance endpoint;
      private readonly IStartableEndpointWithExternallyManagedContainer startableEndpoint;
      private readonly ServiceProviderAdapter serviceProviderAdapter;

      public NServiceBusHostedService(IStartableEndpointWithExternallyManagedContainer startableEndpoint, IServiceProvider serviceProvider)
      {
         this.startableEndpoint = startableEndpoint;
         this.serviceProviderAdapter = new ServiceProviderAdapter(serviceProvider);
      }

      public async Task StartAsync(CancellationToken cancellationToken)
      {
         endpoint = await startableEndpoint.Start(serviceProviderAdapter)
             .ConfigureAwait(false);
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         return endpoint.Stop();
      }

      public void Dispose()
      {
         serviceProviderAdapter?.Dispose();
      }
   }
}
