// <copyright file="BctLog.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using System;
using BCT.Common.Logging.Extensions;
using NServiceBus.Logging;

namespace Bct.Common.NServiceBus.Logging
{
   /// <summary>
   /// The BctLog class.
   /// </summary>
   internal class BctLog : ILog
   {
      private readonly string _name;

      /// <summary>
      /// Initializes a new instance of the <see cref="BctLog"/> class.
      /// </summary>
      /// <param name="name"> The name to log.</param>
      /// <param name="level"> The logging level.</param>
      public BctLog(string name, LogLevel level)
      {
         this._name = name;
         IsDebugEnabled = level <= LogLevel.Debug;
         IsInfoEnabled = level <= LogLevel.Info;
         IsWarnEnabled = level <= LogLevel.Warn;
         IsErrorEnabled = level <= LogLevel.Error;
         IsFatalEnabled = level <= LogLevel.Fatal;
      }

      /// <summary>
      /// Gets a value indicating whether debug is enabled.
      /// </summary>
      /// <value>
      /// Is debug enabled.
      /// </value>
      public bool IsDebugEnabled { get; }

      /// <summary>
      /// Gets a value indicating whether info is enabled.
      /// </summary>
      /// <value>
      /// Is info enabled.
      /// </value>
      public bool IsInfoEnabled { get; }

      /// <summary>
      /// Gets a value indicating whether warning is enabled.
      /// </summary>
      /// <value>
      /// Is warning enabled.
      /// </value>
      public bool IsWarnEnabled { get; }

      /// <summary>
      /// Gets a value indicating whether error is enabled.
      /// </summary>
      /// <value>
      /// Is error enabled.
      /// </value>
      public bool IsErrorEnabled { get; }

      /// <summary>
      /// Gets a value indicating whether debug is enabled.
      /// </summary>
      /// <value>
      /// Is debug enabled.
      /// </value>
      public bool IsFatalEnabled { get; }

      private void Write(string level, string message, Exception exception)
      {
         var logger = BCT.Common.Logging.Extensions.BCTLoggerService.GetLogger<BctLog>();
         if (level == "Debug")
         {
            logger.WithDebug($"{_name}. {level}. {message}. Exception: {exception}").Log();
         }
         else if (level == "Info")
         {
            logger.WithInformation($"{_name}. {level}. {message}. Exception: {exception}").Log();
         }
         else if (level == "Warn")
         {
            logger.WithWarning($"{_name}. {level}. {message}. Exception: {exception}").Log();
         }
         else if (level == "Error")
         {
            logger.WithError($"{_name}. {level}. {message}. Exception: {exception}").Log();
         }
         else if (level == "Fatal")
         {
            logger.WithCritical($"{_name}. {level}. {message}. Exception: {exception}").Log();
         }
      }

      private void Write(string level, string message)
      {
         var logger = BCT.Common.Logging.Extensions.BCTLoggerService.GetLogger<BctLog>();
         if (level == "Debug")
         {
            logger.WithDebug($"{_name}. {level}. {message}.").Log();
         }
         else if (level == "Info")
         {
            logger.WithInformation($"{_name}. {level}. {message}.").Log();
         }
         else if (level == "Warn")
         {
            logger.WithWarning($"{_name}. {level}. {message}.").Log();
         }
         else if (level == "Error")
         {
            logger.WithError($"{_name}. {level}. {message}.").Log();
         }
         else if (level == "Fatal")
         {
            logger.WithCritical($"{_name}. {level}. {message}.").Log();
         }
      }

      private void Write(string level, string format, params object[] args)
      {
         format = $"{_name}. {level}. {format}";

         var logger = BCT.Common.Logging.Extensions.BCTLoggerService.GetLogger<BctLog>();
         if (level == "Debug")
         {
            logger.WithDebug(format, args).Log();
         }
         else if (level == "Info")
         {
            logger.WithInformation(format, args).Log();
         }
         else if (level == "Warn")
         {
            logger.WithWarning(format, args).Log();
         }
         else if (level == "Error")
         {
            logger.WithError(format, args).Log();
         }
         else if (level == "Fatal")
         {
            logger.WithCritical(format, args).Log();
         }
      }

      /// <summary>
      /// Write an debug message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Debug(string message)
      {
         if (IsDebugEnabled)
         {
            Write("Debug", message);
         }
      }

      /// <summary>
      /// Write an debug message with the exception.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="exception">The arguments.</param>
      public void Debug(string message, Exception exception)
      {
         if (IsDebugEnabled)
         {
            Write("Debug", message, exception);
         }
      }

      /// <summary>
      /// Write a debug message with formatted arguments.
      /// </summary>
      /// <param name="format">The format.</param>
      /// <param name="args">The arguments.</param>
      public void DebugFormat(string format, params object[] args)
      {
         if (IsDebugEnabled)
         {
            Write("Debug", format, args);
         }
      }

      /// <summary>
      /// Write an info message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Info(string message)
      {
         if (IsInfoEnabled)
         {
            Write("Info", message);
         }
      }

      /// <summary>
      /// Write an info message.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="exception">The exception.</param>
      public void Info(string message, Exception exception)
      {
         if (IsInfoEnabled)
         {
            Write("Info", message, exception);
         }
      }

      /// <summary>
      /// Write an info message with formatted arguments.
      /// </summary>
      /// <param name="format">The format.</param>
      /// <param name="args">The exception.</param>
      public void InfoFormat(string format, params object[] args)
      {
         if (IsInfoEnabled)
         {
            Write("Info", format, args);
         }
      }

      /// <summary>
      /// Write a warning message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Warn(string message)
      {
         if (IsWarnEnabled)
         {
            Write("Warn", message);
         }
      }

      /// <summary>
      /// Write a warning message with the exception.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="exception">The exception.</param>
      public void Warn(string message, Exception exception)
      {
         if (IsWarnEnabled)
         {
            Write("Warn", message, exception);
         }
      }

      /// <summary>
      /// Write a warning message with formatted arguments.
      /// </summary>
      /// <param name="format">The format.</param>
      /// <param name="args">The arguments.</param>
      public void WarnFormat(string format, params object[] args)
      {
         if (IsWarnEnabled)
         {
            Write("Warn", format, args);
         }
      }

      /// <summary>
      /// Write an error message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Error(string message)
      {
         if (IsErrorEnabled)
         {
            Write("Error", message);
         }
      }

      /// <summary>
      /// Write an error message with the exception.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="exception">The exception.</param>
      public void Error(string message, Exception exception)
      {
         if (IsErrorEnabled)
         {
            Write("Error", message, exception);
         }
      }

      /// <summary>
      /// Write an error message.
      /// </summary>
      /// <param name="format">The message format.</param>
      /// <param name="args">The arguments.</param>
      public void ErrorFormat(string format, params object[] args)
      {
         if (IsErrorEnabled)
         {
            Write("Error", format, args);
         }
      }

      /// <summary>
      /// Write a fatal message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Fatal(string message)
      {
         if (IsFatalEnabled)
         {
            Write("Fatal", message);
         }
      }

      /// <summary>
      /// Write a fatal message with the exception.
      /// </summary>
      /// <param name="message">The message.</param>
      /// <param name="exception">The exception.</param>
      public void Fatal(string message, Exception exception)
      {
         if (IsFatalEnabled)
         {
            Write("Fatal", message, exception);
         }
      }

      /// <summary>
      /// Write a fatal message with formatted arguments.
      /// </summary>
      /// <param name="format">The format.</param>
      /// <param name="args">The arguments.</param>
      public void FatalFormat(string format, params object[] args)
      {
         if (IsFatalEnabled)
         {
            Write("Fatal", format, args);
         }
      }
   }
}