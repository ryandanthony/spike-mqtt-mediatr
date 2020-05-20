// <copyright file="IBarcodeServiceFactory.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System.Data;
using Microsoft.Extensions.Logging;

namespace Bct.Barcode
{
   /// <summary>
   /// The IBarcodeServiceFactory interface.
   /// </summary>
   public interface IBarcodeServiceFactory
   {
      /// <summary>
      /// The entry point for the application.
      /// </summary>
      /// <param name="logger"> The logger.</param>
      /// <param name="transaction"> The database transaction.</param>
      /// <returns>The barcode service.</returns>
      IBarcodeService Create(ILogger<BarcodeService> logger, IDbTransaction transaction);
   }
}
