using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Threading;
using CommandLine;
using NLog;
using QuickCopy.OptionModels;
using QuickCopy.Utilities;

namespace QuickCopy
{
    internal class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            Run(args);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void Run(IEnumerable<string> args)
        {
            var parsedArguments =
                Parser.Default.ParseArguments<ProgramOptions>(args);

            parsedArguments.WithParsed(o =>
            {
                switch (o.Runtime)
                {
                    case RuntimeType.Console:
                        RunConsoleMode(o);
                        break;
                    case RuntimeType.Service:
                        ServiceBase.Run(new WindowsService(o));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private static void RunConsoleMode(ProgramOptions opts)
        {
            Log.Info("Run in console mode.");
            Log.Info("Press 'q' to quit, and any other key to continue.");
            while (true)
            {
                opts.RunIncrementalCopy();
                Log.Info($"Waiting {opts.CheckTime}ms to continue.");
                Thread.Sleep(Convert.ToInt32(opts.CheckTime));
            }
            // Log.Info("Quitting console mode.");
        }
    }
}