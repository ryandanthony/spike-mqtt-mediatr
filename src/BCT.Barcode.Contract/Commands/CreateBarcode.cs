// <copyright file="CreateBarcode.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

namespace Bct.Barcode.Contract.Commands
{
   /// <summary>
   /// The CreateBarcode command.
   /// </summary>
   public class CreateBarcode
   {
      /// <summary>
      /// Gets or sets the barcode Name.
      /// </summary>
      /// <value>
      /// The barcode Name.
      /// </value>
      public string BarcodeName { get; set; }
   }
}
