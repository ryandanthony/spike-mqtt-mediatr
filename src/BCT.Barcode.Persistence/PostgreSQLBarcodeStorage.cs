// <copyright file="PostgreSQLBarcodeStorage.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BCT.Common.Logging.Extensions;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bct.Barcode.Persistence
{
   /// <summary>
   /// PostgreSQL persistent storage.
   /// </summary>
   public class PostgreSQLBarcodeStorage : IBarcodeStorage
   {
      private ILogger<PostgreSQLBarcodeStorage> _logger;
      private IDbConnection _connection;
      private IDbTransaction _transaction;

      /// <summary>
      /// Initializes a new instance of the <see cref="PostgreSQLBarcodeStorage"/> class.
      /// </summary>
      /// <param name="logger">The logger.</param>
      /// <param name="transaction">The database transaction.</param>
      public PostgreSQLBarcodeStorage(ILogger<PostgreSQLBarcodeStorage> logger, IDbTransaction transaction)
      {
         if (transaction == null)
         {
            throw new ArgumentNullException(nameof(transaction));
         }

         _logger = logger;
         _logger.WithDebug("Entering PostgreSQLBarcodeStorage constructor.").Log();
         _connection = transaction.Connection;
         _transaction = transaction;
      }

      /// <summary>
      /// Inserts a <see cref="Barcode"/> into the database.
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

         var sql = @"insert into Barcodes
                        (Id, BarcodeName)
            values      (@Id, @Name)";

         _connection.Execute(sql: sql, transaction: _transaction, param: barcode);

         return Task.CompletedTask;
      }

      /// <summary>
      /// Gets all <see cref="Barcode"/> from the database.
      /// </summary>
      /// <returns>A collection of <see cref="Barcode"/> objects.</returns>
      public IList<Entities.Barcode> GetAll()
      {
         _logger.WithDebug("Getting entire list of Barcodes from database.").Log();

         var sql = @"SELECT Id, BarcodeName as Name FROM Barcodes";

         var barcodeList = _connection.Query<Entities.Barcode>(sql: sql).ToList();

         return barcodeList;
      }

      /// <summary>
      /// Gets a barcode from the database.
      /// </summary>
      /// <returns>An object <see cref="Barcode"/>.</returns>
      /// <param name="id"> Id of barcode.</param>
      public Entities.Barcode GetById(long id)
      {
         _logger.WithDebug("Getting barcode with id, {GUID}", id).Log();

         var sql = @"SELECT Id, BarcodeName as Name FROM Barcodes WHERE Id = @Id";
         var p = new DynamicParameters();
         p.Add("Id", id, DbType.Int64);

         var barcodeList = _connection.Query<Entities.Barcode>(sql: sql, param: p);
         if (barcodeList.Any())
         {
            return barcodeList.First();
         }
         else
         {
            return null;
         }
      }
   }
}
