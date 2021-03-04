using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Threading;
using CommandLine;
using NLog;
using QuickCopy.Configuration;

namespace QuickCopy
{
    internal class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var programDirectory = Environment.CurrentDirectory;
            var header = new Header();
            header.Print(Log, programDirectory);
            Run(args);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void Run(IEnumerable<string> args)
        {
            var parsedArguments =
                Parser.Default.ParseArguments<ProgramOptions>(args);

            parsedArguments.WithParsed(o =>
            {
                if (o.UseConfigFile)
                {
                    var configFile = ConfigExtensions.BuildConfigurationFile(Environment.CurrentDirectory);
                    PickRuntime(configFile);
                }
                else
                {
                    PickRuntime(o);
                }
            });
        }

        private static void PickRuntime(ProgramOptions o)
        {
            switch (o.Runtime)
            {
                case RuntimeType.Console:
                    RunConsoleMode(o);
                    break;
                case RuntimeType.Service:
                    ServiceBase.Run(new WindowsService(o));
                    break;
                case RuntimeType.Batch:
                    RunBatchMode(o);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ConsoleKeyListener(object obj)
        {
            var token = (CancellationTokenSource) obj;
            while (true)
            {
                if (Console.ReadKey().Key != ConsoleKey.Q) continue;
                token.Cancel(false);
                Log.Info("Q key pressed, waiting for job to finish before cancelling.");
                break;
            }
        }

        private static void RunConsoleMode(ProgramOptions opts)
        {
            Log.Info("Run in console mode.");
            Log.Info("Press 'q' to quit, and any other key to continue.");

            var cts = new CancellationTokenSource();
            var t = new Thread(ConsoleKeyListener);
            t.Start(cts);

            while (!cts.Token.IsCancellationRequested)
            {
                opts.RunIncrementalCopy();
                Log.Info($"Waiting {opts.CheckTime}ms to continue.");
                Thread.Sleep(Convert.ToInt32(opts.CheckTime));
            }

            Log.Info("Exiting console mode");
        }

        private static void RunBatchMode(ProgramOptions opts)
        {
            Log.Info("Run in batch mode.");
            opts.RunIncrementalCopy();
            Log.Info("Batch completed");
        }
    }
}