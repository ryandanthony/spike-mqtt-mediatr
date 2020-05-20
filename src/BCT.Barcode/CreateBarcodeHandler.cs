// <copyright file="CreateBarcodeHandler.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Data;
using System.Threading.Tasks;
using Bct.Barcode.Contract.Commands;
using Bct.Barcode.Contract.Events;
using Bct.Barcode.Contract.Messages;
using BCT.Common.Logging.Extensions;
using Bct.Common.NServiceBus.Tracing;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Prometheus;

namespace Bct.Barcode
{
   /// <summary>
   /// The CreateBarcodeHandler class.
   /// </summary>
   public class CreateBarcodeHandler : IHandleMessages<CreateBarcode>
   {
      private readonly ILogger<CreateBarcodeHandler> _logger;
      private readonly IBarcodeServiceFactory _factory;

      /// <summary>
      /// Initializes a new instance of the <see cref="CreateBarcodeHandler"/> class.
      /// </summary>
      /// <param name="logger"> The logger.</param>
      /// <param name="factory"> The BarcodeService factory.</param>
      public CreateBarcodeHandler(ILogger<CreateBarcodeHandler> logger, IBarcodeServiceFactory factory)
      {
         _logger = logger;
         _factory = factory;
      }

      /// <summary>
      /// The entry point for the application.
      /// </summary>
      /// <param name="message"> The message.</param>
      /// <param name="context"> The context of the message.</param>
      /// <returns>A task object.</returns>
      public async Task Handle(CreateBarcode message, IMessageHandlerContext context)
      {
         if (message == null)
         {
            throw new ArgumentNullException(nameof(message));
         }

         if (context == null)
         {
            throw new ArgumentNullException(nameof(context));
         }

         //span.Log("this is the name:" + message.BarcodeName);
         _logger.WithInformation("CreateBarcodeHandler - Received CreateBarcode message with messageId = {MessageId}, with BarcodeName = {BarcodeName}", context.MessageId, message.BarcodeName).Log();

         IDbTransaction transaction = context.SynchronizedStorageSession?.SqlPersistenceSession()?.Transaction;
         IBarcodeService service = _factory.Create(BCTLoggerService.GetLogger<BarcodeService>(), transaction);

         var barcode = service.CreateBarcode(message.BarcodeName);
         var span = context.GetSpan();
         span.SetTag("Barcode.Id", barcode.Id);
         span.SetTag("Barcode.Name", barcode.Name);
         await context.Publish(new BarcodeCreated() { Id = barcode.Id, Name = barcode.Name }).ConfigureAwait(false);
         _logger.WithInformation("CreateBarcodeHandler - Published created barcode from message with messageid = {MessageId}, with BarcodeName = {BarcodeName}.", context.MessageId, message.BarcodeName).Log();

         await context.Reply(new CreateBarcodeResponse() { CreatedBarcodeId = barcode.Id }).ConfigureAwait(false);
         _logger.WithInformation("CreateBarcoderHandler - Reply with messageId = {MessageId} with barcode id, {GUID}.", context.MessageId, barcode.Id).Log();
      }
   }
}