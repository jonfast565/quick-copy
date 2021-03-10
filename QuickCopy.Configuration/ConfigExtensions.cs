using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using NLog;

namespace QuickCopy.Configuration
{
    public static class ConfigExtensions
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static ProgramOptions BuildConfigurationFile(string workingDirectory)
        {
            var args = Environment.GetCommandLineArgs();

            // get and set environment
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environment)) environment = "Development";

            Log.Info($"Environment is: {environment}");

            // config
            var config = new ConfigurationBuilder()
                .SetBasePath(workingDirectory)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environment}.json", false)
                .Build();

            var result = new ProgramOptions
            {
                Runtime = (RuntimeType)Enum.Parse(typeof(RuntimeType), config["Runtime"], true),
                SourceDirectory = config["SourceDirectory"],
                TargetDirectories = config.GetSection("TargetDirectory")
                    .AsEnumerable()
                    .Select(x => x.Value)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList(),
                CheckTime = config.GetValue<double>("CheckTime"),
                EnableDeletes = config.GetValue<bool>("EnableDeletes"),
                SkipFolders = config.GetSection("SkipFolders")
                    .AsEnumerable()
                    .Select(x => x.Value)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList(),
                UseConfigFile = true
            };

            if (string.IsNullOrEmpty(result.SourceDirectory))
            {
                throw new Exception("Source directory not set");
            }

            if (!result.TargetDirectories.Any() || result.TargetDirectories.Any(string.IsNullOrEmpty))
            {
                throw new Exception("Target directories not set, or one is an empty string/null value");
            }

            result.SkipFolders ??= new string[] { };

            return result;
        }
    }
}
