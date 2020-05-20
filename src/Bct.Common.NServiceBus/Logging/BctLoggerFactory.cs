// <copyright file="BctLoggerFactory.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using NServiceBus.Logging;

namespace Bct.Common.NServiceBus.Logging
{

   /// <summary>
   /// The BctLoggerFactory class.
   /// </summary>
   internal class BctLoggerFactory : ILoggerFactory
   {
      private readonly LogLevel _level;

      /// <summary>
      /// Initializes a new instance of the <see cref="BctLoggerFactory"/> class.
      /// </summary>
      /// <param name="level">The LogLevel level.</param>
      public BctLoggerFactory(LogLevel level)
      {
         _level = level;
      }

      /// <summary>
      /// Get the logger.
      /// </summary>
      /// <param name="type"> The type of the log.</param>
      /// <returns>The logger.</returns>
      public ILog GetLogger(Type type)
      {
         if (type == null)
         {
            throw new ArgumentNullException(nameof(type));
         }

         return GetLogger(type.FullName);
      }

      /// <summary>
      /// Get the logger.
      /// </summary>
      /// <param name="name"> The name of the log.</param>
      /// <returns>The logger.</returns>
      public ILog GetLogger(string name)
      {
         return new BctLog(name, _level);
      }
   }
}
