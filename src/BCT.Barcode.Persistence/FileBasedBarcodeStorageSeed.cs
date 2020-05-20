// <copyright file="FileBasedBarcodeStorageSeed.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;

namespace Bct.Persistence
{
   /// <summary>
   /// FileBasedBarcodeStorageSeed class.
   /// </summary>
   public class FileBasedBarcodeStorageSeed
   {
      /// <summary>
      /// Gets or sets the barcode Id.
      /// </summary>
      /// <value>
      /// The barcode Id.
      /// </value>
      public int Id { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether the storage has been seeded.
      /// </summary>
      /// <value>
      /// A value indicating whether the storage has been seeded.
      /// </value>
      public bool Seeded { get; set; }

      /// <summary>
      /// Gets or sets the barcode SeededTime property.
      /// </summary>
      /// <value>
      /// The barcode SeededTime property.
      /// </value>
      public DateTime SeededTime { get; set; }
   }
}
