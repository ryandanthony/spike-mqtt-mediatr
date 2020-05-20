// <copyright file="GetBarcodesResponse.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Bct.Barcode.Contract.Messages
{
   /// <summary>
   /// GetBarcodesResponse message.
   /// </summary>
   public class GetBarcodesResponse
   {
      /// <summary>
      /// Gets or sets the list of Barcodes.
      /// </summary>
      /// <value>
      /// The list of Barcodes.
      /// </value>
      public IList<Contract.Barcode> Barcodes { get; set; }
   }
}
