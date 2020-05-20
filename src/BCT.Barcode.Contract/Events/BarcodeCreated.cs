// <copyright file="BarcodeCreated.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

namespace Bct.Barcode.Contract.Events
{
   /// <summary>
   /// BarcodeCreated event.
   /// </summary>
   public class BarcodeCreated
   {
      /// <summary>
      /// Gets or sets the barcode Id.
      /// </summary>
      /// <value>
      /// The barcode Id.
      /// </value>
      public long Id { get; set; }

      /// <summary>
      /// Gets or sets the barcode Name.
      /// </summary>
      /// <value>
      /// The barcode Name.
      /// </value>
      public string Name { get; set; }
   }
}
