// <copyright file="Barcode.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

namespace Bct.Barcode.Persistence.Entities
{
   /// <summary>
   /// Barcode entity.
   /// </summary>
   public class Barcode
   {
      /// <summary>
      /// Gets or sets the Id.
      /// </summary>
      /// <value>
      /// The Id.
      /// </value>
      public long Id { get; set; }

      /// <summary>
      /// Gets or sets the Name.
      /// </summary>
      /// <value>
      /// The Name.
      /// </value>
      public string Name { get; set; }
   }
}