// <copyright file="BctLoggerDefinition.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using NServiceBus.Logging;

namespace Bct.Common.NServiceBus.Logging
{
   /// <summary>
   /// The BctLoggerDefinition class.
   /// </summary>
   internal class BctLoggerDefinition : LoggingFactoryDefinition
   {
      private LogLevel _level = LogLevel.Info;

      /// <summary>
      /// Initializes a new instance of the <see cref="BctLoggerDefinition"/> class.
      /// </summary>
      /// <param name="level"> The LogLevel level.</param>
      public void Level(LogLevel level)
      {
         this._level = level;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BctLoggerDefinition"/> class.
      /// </summary>
      /// <param name="logLevel"> The logLevel string.</param>
      public void Level(string logLevel)
      {
         LogLevel level = LogLevel.Debug;

         switch (logLevel)
         {
            case "Verbose":
               level = LogLevel.Debug;
               break;
            case "Debug":
               level = LogLevel.Debug;
               break;
            case "Warning":
               level = LogLevel.Warn;
               break;
            case "Information":
               level = LogLevel.Info;
               break;
            case "Error":
               level = LogLevel.Error;
               break;
            case "Fatal":
               level = LogLevel.Fatal;
               break;
         }

         this._level = level;
      }

      /// <summary>
      /// Get the logging factory..
      /// </summary>
      /// <returns>The logging factory.</returns>
      protected override ILoggerFactory GetLoggingFactory()
      {
         return new BctLoggerFactory(_level);
      }
   }
}