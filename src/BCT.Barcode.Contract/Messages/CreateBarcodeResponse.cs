// <copyright file="CreateBarcodeResponse.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

namespace Bct.Barcode.Contract.Messages
{
   /// <summary>
   /// CreateBarcodeResponse message.
   /// </summary>
   public class CreateBarcodeResponse
   {
      /// <summary>
      /// Gets or sets the CreatedBarcodeId.
      /// </summary>
      /// <value>
      /// The Created Barcode Id.
      /// </value>
      public long CreatedBarcodeId { get; set; }
   }
}
