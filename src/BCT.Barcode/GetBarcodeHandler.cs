// <copyright file="GetBarcodeHandler.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Data;
using System.Threading.Tasks;
using Bct.Barcode.Contract.Messages;
using Bct.Barcode.Contract.Queries;
using BCT.Common.Logging.Extensions;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace Bct.Barcode
{
   /// <summary>
   /// The GetBarcodeHandler class.
   /// </summary>
   public class GetBarcodeHandler : IHandleMessages<GetBarcode>
   {
      private readonly ILogger<GetBarcodeHandler> _logger;
      private readonly IBarcodeServiceFactory _factory;

      /// <summary>
      /// Initializes a new instance of the <see cref="GetBarcodeHandler"/> class.
      /// </summary>
      /// <param name="logger">The logger.</param>
      /// <param name="factory">The factory.</param>
      public GetBarcodeHandler(ILogger<GetBarcodeHandler> logger, IBarcodeServiceFactory factory)
      {
         _logger = logger;
         _factory = factory;
      }

      /// <summary>
      /// The GetBarcode handler.
      /// </summary>
      /// <param name="message"> The message.</param>
      /// <param name="context"> The current context.</param>
      /// <returns>A task object.</returns>
      public async Task Handle(GetBarcode message, IMessageHandlerContext context)
      {
         if (message == null)
         {
            throw new ArgumentNullException(nameof(message));
         }

         if (context == null)
         {
            throw new ArgumentNullException(nameof(context));
         }

         _logger.WithInformation(
             "GetBarcodeHandler - Request message with MessageId = {MessageID}, retrieving get barcode by id {GUID} received.",
             context.MessageId,
             message.Id)
            .Log();

         IDbTransaction transaction = context.SynchronizedStorageSession.SqlPersistenceSession().Transaction;
         IBarcodeService service = _factory.Create(BCTLoggerService.GetLogger<BarcodeService>(), transaction);

         var barcode = service.GetBarcodeById(message.Id);

         await context.Reply(new GetBarcodeResponse() { Barcode = barcode }).ConfigureAwait(false);
         _logger.WithInformation("GetBarcodeHandler - Replies with messageId = {MessageID}", context.MessageId);
      }
   }
}