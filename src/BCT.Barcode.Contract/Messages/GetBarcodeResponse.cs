// <copyright file="GetBarcodeResponse.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

namespace Bct.Barcode.Contract.Messages
{
   /// <summary>
   /// GetBarcodeResponse message.
   /// </summary>
   public class GetBarcodeResponse
   {
      /// <summary>
      /// Gets or sets the barcode.
      /// </summary>
      /// <value>
      /// The barcode.
      /// </value>
      public Barcode Barcode { get; set; }
   }
}
