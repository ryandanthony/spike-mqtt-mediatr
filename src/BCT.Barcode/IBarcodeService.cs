// <copyright file="IBarcodeService.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Bct.Barcode
{
   /// <summary>
   /// The IBarcodeService interface.
   /// </summary>
   public interface IBarcodeService
   {
      /// <summary>
      /// Create a new barcode.
      /// </summary>
      /// <param name="name"> The name of the barcode.</param>
      /// <returns>A barcode.</returns>
      Contract.Barcode CreateBarcode(string name);

      /// <summary>
      /// The entry point for the application.
      /// </summary>
      /// <param name="id"> The id of the barcode.</param>
      /// <returns>A task object.</returns>
      /// <returns>A barcode.</returns>
      Contract.Barcode GetBarcodeById(long id);

      /// <summary>
      /// Get all barcodes.
      /// </summary>
      /// <returns>A list of barcodes.</returns>
      IList<Contract.Barcode> GetAll();
   }
}
