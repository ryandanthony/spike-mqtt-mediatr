// <copyright file="FileBasedBarcodeStorage.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCT.Common.Logging.Extensions;
using Bct.Persistence;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Bct.Barcode.Persistence
{
   /// <summary>
   /// FileBasedBarcodeStorage persistent storage.
   /// </summary>
   public class FileBasedBarcodeStorage : IBarcodeStorage
   {
      private ILogger<FileBasedBarcodeStorage> _logger;
      private readonly string _dbName;

      /// <summary>
      /// Initializes a new instance of the <see cref="FileBasedBarcodeStorage"/> class.
      /// </summary>
      /// <param name="dbName">The database name.</param>
      /// <param name="logger">The logger.</param>
      public FileBasedBarcodeStorage(string dbName, ILogger<FileBasedBarcodeStorage> logger)
      {
         _logger = logger;
         _logger.WithDebug("Entering FileBasedBarcodeStorage constructor provided name, {Name}.", dbName).Log();
         _dbName = dbName;
         Seed();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="FileBasedBarcodeStorage"/> class.
      /// </summary>
      /// <param name="logger">The logger.</param>
      public FileBasedBarcodeStorage(ILogger<FileBasedBarcodeStorage> logger)
      {
         _logger = logger;
         _logger.WithDebug("Entering FileBasedBarcodeStorage constructor provided name, {Name}.", "FileBasedBarcodeStorage").Log();
         _dbName = "FileBasedBarcodeStorage";
         Seed();
      }

      private void Seed()
      {
         using (var db = new LiteDatabase(_dbName))
         {
            _logger.WithInformation("Seeding the storage.").Log();
            var seedCollection = db.GetCollection<FileBasedBarcodeStorageSeed>("seed");

            // Do not seed an already seeded database
            if (seedCollection.Count() > 0)
            {
               _logger.WithWarning("Storage already has seed.").Log();
               return;
            }

            seedCollection.Insert(new FileBasedBarcodeStorageSeed()
            {
               Seeded = true,
               SeededTime = DateTime.Now,
            });

            _logger.WithDebug("Loading test barcodes into storage barcode collection.").Log();
            var barcodes = GetBarcodeCollection(db);

            barcodes.Insert(new Entities.Barcode()
            {
               Id = 1,
               Name = "TestBarcode1",
            });

            barcodes.Insert(new Entities.Barcode()
            {
               Id = 2,
               Name = "TestBarcode2",
            });

            barcodes.Insert(new Entities.Barcode()
            {
               Id = 3,
               Name = "TestBarcode3",
            });
         }
      }

      /// <summary>
      /// Inserts a <see cref="Barcode"/> into the file storage.
      /// </summary>
      /// <param name="barcode">The barcode to be inserted.</param>
      /// <returns>The newly created <see cref="Barcode"/> instance.
      /// <remarks>
      /// If no Id was supplied to the barcode, the Id will be populated
      /// </remarks>
      /// </returns>
      public Task InsertAsync(Entities.Barcode barcode)
      {
         if (barcode == null)
         {
            throw new ArgumentNullException(nameof(barcode));
         }

         _logger.WithDebug("Inserting barcode with id, {GUID}", barcode.Id).Log();
         using (var db = new LiteDatabase(_dbName))
         {
            var barcodesCollection = GetBarcodeCollection(db);
            barcodesCollection.Insert(barcode);
            return Task.CompletedTask;
         }
      }

      /// <summary>
      /// Gets all <see cref="Barcode"/> from the file storage.
      /// </summary>
      /// <returns>A collection of <see cref="Barcode"/> objects.</returns>
      public IList<Entities.Barcode> GetAll()
      {
         _logger.WithDebug("Getting entire list of Barcodes from storage {StorageName}.", _dbName).Log();
         using (var db = new LiteDatabase(_dbName))
         {
            var barcodesCollection = GetBarcodeCollection(db);
            return barcodesCollection.FindAll().ToList();
         }
      }

      /// <summary>
      /// Get barcode by Id.
      /// </summary>
      /// <param name="id"> The id.</param>
      /// <returns>A barcode.</returns>
      public Entities.Barcode GetById(long id)
      {
         _logger.WithDebug("Getting barcode from {StorageName} with id, {GUID}", _dbName, id).Log();
         using (var db = new LiteDatabase(_dbName))
         {
            var barcodesCollection = GetBarcodeCollection(db);
            return barcodesCollection.FindById(id);
         }
      }

      /// <summary>
      /// Helper method for method encapsulation.
      /// </summary>
      /// <param name="db">The instance of the LiteDatabase being queried.</param>
      /// <returns>A collection associated with barcode storage.</returns>
      private ILiteCollection<Entities.Barcode> GetBarcodeCollection(LiteDatabase db)
      {
         return db.GetCollection<Entities.Barcode>("barcodes");
      }
   }
}
