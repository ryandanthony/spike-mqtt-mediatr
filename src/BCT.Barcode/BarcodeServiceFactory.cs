// <copyright file="BarcodeServiceFactory.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Data;
using Bct.Barcode.Persistence;
using Microsoft.Extensions.Logging;

namespace Bct.Barcode
{
   /// <summary>
   /// The BarcodeService factory.
   /// </summary>
   public class BarcodeServiceFactory : IBarcodeServiceFactory
   {
      private readonly ILoggerFactory _loggerFactory;
      private readonly string _persistenceType;

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeServiceFactory"/> class.
      /// </summary>
      /// <param name="loggerFactory"> The logger factory.</param>
      /// <param name="persistenceType"> The persistence type.</param>
      public BarcodeServiceFactory(ILoggerFactory loggerFactory, string persistenceType)
      {
         _loggerFactory = loggerFactory;
         _persistenceType = persistenceType;
      }

      /// <summary>
      /// Create the persistent storage.
      /// </summary>
      /// <param name="logger"> The logger.</param>
      /// <param name="transaction"> The database transaction.</param>
      /// <returns>
      /// The storage instance.
      /// </returns>
      public IBarcodeService Create(ILogger<BarcodeService> logger, IDbTransaction transaction)
      {
         IBarcodeStorage storage = null;
         switch (_persistenceType)
         {
            case "PostgreSQL":
               storage = new PostgreSQLBarcodeStorage(_loggerFactory.CreateLogger<PostgreSQLBarcodeStorage>(), transaction);
               break;
            case "FileBased":
               storage = new FileBasedBarcodeStorage("FileBasedBarcodeStorage", _loggerFactory.CreateLogger<FileBasedBarcodeStorage>());
               break;
            default:
               throw new NotImplementedException();
         }

         return new BarcodeService(_loggerFactory.CreateLogger<BarcodeService>(), storage);
      }
   }
}
