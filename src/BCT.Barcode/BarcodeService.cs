// <copyright file="BarcodeService.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bct.Barcode.Persistence;
using BCT.Common.Logging.Extensions;
using IdGen;
using Microsoft.Extensions.Logging;

namespace Bct.Barcode
{
   /// <summary>
   /// The BarcodeService.
   /// </summary>
   public class BarcodeService : IBarcodeService
   {
      private ILogger<BarcodeService> _logger;
      private readonly IBarcodeStorage _storage;
      private readonly IdGenerator _snowflake;

      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeService"/> class.
      /// </summary>
      /// <param name="logger"> The logger.</param>
      /// <param name="storage"> The storage.</param>
      public BarcodeService(ILogger<BarcodeService> logger, IBarcodeStorage storage)
      {
         _logger = logger;
         _snowflake = new IdGenerator(1);
         _storage = storage;
      }

      /// <summary>
      /// Create a Barcode.
      /// </summary>
      /// <param name="name"> The name of the barcode.</param>
      /// <returns>
      /// The new barcode.
      /// </returns>
      public Contract.Barcode CreateBarcode(string name)
      {
         var barcode = new Persistence.Entities.Barcode()
         {
            Id = _snowflake.CreateId(),
            Name = name,
         };
         _logger.WithInformation("Created new Barcode with BarcodeName = {BarcodeName} and Id = {GUID}", barcode.Name, barcode.Id).Log();

         _ = _storage.InsertAsync(barcode);
         return new Contract.Barcode() { Id = barcode.Id, Name = barcode.Name, };
      }

      /// <summary>
      /// Get barcode by Id.
      /// </summary>
      /// <param name="id"> The barcode id.</param>
      /// <returns>
      /// The barcode.
      /// </returns>
      public Contract.Barcode GetBarcodeById(long id)
      {
         var barcode = _storage.GetById(id);

         if (barcode == null)
         {
            _logger.WithWarning("Could not retrieve barcode with id = {GUID}", id).Log();
            return null;
         }

         _logger.WithInformation("Barcode service returns barcode with BarcodeName = {BarcodeName} and id = {GUID}", barcode.Name, barcode.Id).Log();
         return new Contract.Barcode() { Id = barcode.Id, Name = barcode.Name };
      }

      /// <summary>
      /// Get all barcodes.
      /// </summary>
      /// <returns>
      /// A list of barcodes.
      /// </returns>
      public IList<Contract.Barcode> GetAll()
      {
         List<Contract.Barcode> barcodes = _storage.GetAll()
             .Select(barcode => new Contract.Barcode { Id = barcode.Id, Name = barcode.Name }).ToList();
         return barcodes;
      }

   }
}
