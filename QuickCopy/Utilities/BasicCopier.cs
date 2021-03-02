using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using NLog;
using QuickCopy.OptionModels;
using QuickCopy.PathModels;

namespace QuickCopy.Utilities
{
    public class BasicCopier
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public BasicCopier(ProgramOptions opts)
        {
            Options = opts;
        }

        public ProgramOptions Options { get; set; }

        public void Copy()
        {
            DirectoryCopy(Options.SourceDirectory, Options.TargetDirectory, true, true);
        }

        public void IncrementalCopy(List<FileInfoParserAction> actions)
        {
            var orderedActions = 
                actions.OrderByDescending(x => x.GetSourceOrDestinationLength());

            foreach (var action in orderedActions)
            {
                switch (action.Type)
                {
                    case ActionType.Create:
                    case ActionType.Update:
                        if (action.ParserSource.IsFile)
                        {
                            File.Copy(action.ParserSource.File.FullName, action.ParserDestination.File.FullName, true);
                        }
                        else
                        {
                            DirectoryCopy(action.ParserSource.Directory.FullName, action.ParserDestination.Directory.FullName, true, false);
                        }

                        break;
                    case ActionType.Delete:
                        if (action.ParserDestination.IsFile)
                        {
                            File.Delete(action.ParserDestination.File.FullName);
                        }
                        else
                        {
                            Directory.Delete(action.ParserDestination.Directory.FullName, true);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool recursive, bool cleanFolder)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var destDir = new DirectoryInfo(destDirName);

            if (!dir.Exists)
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);

            if (cleanFolder)
            {
                Directory.Delete(destDir.FullName, true);
                Directory.CreateDirectory(destDir.FullName);
            }

            var dirs = dir.GetDirectories();

            Directory.CreateDirectory(destDirName);

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
                Log.Info($"Copied {tempPath}");
            }

            if (!recursive) return;

            foreach (var subDirectory in dirs)
            {
                var tempPath = Path.Combine(destDirName, subDirectory.Name);
                DirectoryCopy(subDirectory.FullName, tempPath, true, false);
            }
        }
    }
}