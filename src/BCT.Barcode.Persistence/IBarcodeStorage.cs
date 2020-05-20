// <copyright file="IBarcodeStorage.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bct.Barcode.Persistence
{
   /// <summary>
   /// IBarcodeStorage interface.
   /// </summary>
   public interface IBarcodeStorage
   {
      /// <summary>
      /// Insert a new barcode.
      /// </summary>
      /// <param name="barcode"> The barcode to insert.</param>
      /// <returns>A task object.</returns>
      Task InsertAsync(Entities.Barcode barcode);

      /// <summary>
      /// GetAll barcodes.
      /// </summary>
      /// <returns>A list of barcodes.</returns>
      // TODO: Not production ready - need to set filtering, paging, etc. plus not the entire entity perhaps. Fine for POC.
      IList<Entities.Barcode> GetAll();

      /// <summary>
      /// Get barcode by Id.
      /// </summary>
      /// <param name="id"> The id of the barcode.</param>
      /// <returns>A barcode.</returns>
      Entities.Barcode GetById(long id);
   }
}
