// <copyright file="GetBarcodesHandler.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Bct.Barcode.Contract.Messages;
using Bct.Barcode.Contract.Queries;
using BCT.Common.Logging.Extensions;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Prometheus;

namespace Bct.Barcode
{
   /// <summary>
   /// The GetBarcodesHandler class.
   /// </summary>
   public class GetBarcodesHandler : IHandleMessages<GetBarcodes>
   {
      private ILogger<GetBarcodesHandler> _logger;
      private readonly IBarcodeServiceFactory _factory;

      /// <summary>
      /// That is an example of creating a metric in the handler.
      /// </summary>
      private static readonly Gauge _metricGuage = Metrics.CreateGauge(
         name: "bct_target_quantity",
         help: "Track the total quantity of a target.",
         labelNames: new string[] { "service", "targetType" });

      /// <summary>
      /// Initializes a new instance of the <see cref="GetBarcodesHandler"/> class.
      /// </summary>
      /// <param name="logger">The logger.</param>
      /// <param name="factory">The factory.</param>
      public GetBarcodesHandler(ILogger<GetBarcodesHandler> logger, IBarcodeServiceFactory factory)
      {
         _logger = logger;
         _factory = factory;
      }

      /// <summary>
      /// The GetBarcodes handler.
      /// </summary>
      /// <param name="message"> The message.</param>
      /// <param name="context"> The context.</param>
      /// <returns>A task object.</returns>
      public async Task Handle(GetBarcodes message, IMessageHandlerContext context)
      {
         if (context == null)
         {
            throw new ArgumentNullException(nameof(context));
         }
         _metricGuage.WithLabels("GetBarcodesHandler", "Handle").Inc();

         _logger.WithInformation(
            "GetBarcodesHandler - Received GetBarcodes message with messageId = {MessageId}",
            context.MessageId).Log();

         IDbTransaction transaction = context.SynchronizedStorageSession.SqlPersistenceSession().Transaction;
         IBarcodeService service = _factory.Create(BCTLoggerService.GetLogger<BarcodeService>(), transaction);

         List<Contract.Barcode> barcodes = service.GetAll().ToList();

         await context.Reply(new GetBarcodesResponse() { Barcodes = barcodes }).ConfigureAwait(false);
         _logger.WithInformation("GetBarcodesHandler - Reply with messageId = {MessageId}.", context.MessageId)
             .Log();

         _metricGuage.WithLabels("GetBarcodesHandler", "Handle").Dec();
      }
   }
}
