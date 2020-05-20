// <copyright file="ConfigurationBuilderExtensions.cs" company="TerumoBCT">
// Copyright (c) TerumoBCT. All rights reserved.
// </copyright>

using BCT.Common.Logging.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bct.Common.NServiceBus
{
   /// <summary>
   /// ConfigurationBuilder extensions.
   /// </summary>
   public static class ConfigurationBuilderExtensions
   {
      /// <summary>
      /// Adds a standard set of configuration sources and settings to the IConfigurationBuilder.
      /// Allows for specifying a non-secret set of key-file settings and secret set of key-file settings.
      /// Includes SetBasePath, AddKeyPerFileSet, AddEnvironmentVariables as sources. Also logs
      /// all configuration settings to console and system log.
      /// </summary>
      /// <param name="configBuilder">The IConfigurationBuilder being applied to.</param>
      /// <param name="configLog">A StringBuilder that collects all configuration settigs as they are applied.</param>
      /// <param name="configSources">Array of directories of non-secret key-file settings.</param>
      /// <param name="secretSources">Array of directories of secret key-file settings.</param>
      /// <returns>The IConfigurationBuilder returned for fluent operations.</returns>
      public static IConfigurationBuilder AddBctConfigurationSources(this IConfigurationBuilder configBuilder, StringBuilder configLog, string[] configSources, string[] secretSources)
      {
         new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddKeyPerFileSet(configSources)
                     .AddEnvironmentVariables()
                     .AddAndWriteToConsole(configBuilder, redactFields: false, configurationLogging: configLog);

         new ConfigurationBuilder()
                     .AddKeyPerFileSet(secretSources)
                     .AddAndWriteToConsole(configBuilder, redactFields: true, configurationLogging: configLog);

         return configBuilder;
      }
   }
}
