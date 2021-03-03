using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var skipFolder = new PathParser(Options.SkipFolder);

            var orderedCreates =
                actions
                    .Where(x => x.Type == ActionType.Create || x.Type == ActionType.Update)
                    .OrderBy(x => x.GetSourceLength());

            var orderedDeletes =
                actions
                    .Where(x => x.Type == ActionType.Delete)
                    .OrderByDescending(x => x.GetDestinationLength());

            foreach (var action in orderedCreates)
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (action.Type)
                {
                    case ActionType.Create:
                        var destinationSegment = GetDestinationFromSegment(action);
                        if (action.Source.Segment.Contains(skipFolder.Segment))
                            break;

                        if (action.Source.IsFile)
                            File.Copy(action.Source.File.FullName, destinationSegment, true);
                        else
                            DirectoryCopy(action.Source.Directory.FullName, destinationSegment, true, false);
                        break;
                    case ActionType.Update:
                        if (action.Source.Segment.Contains(skipFolder.Segment))
                            break;

                        if (action.Source.IsFile)
                            File.Copy(action.Source.File.FullName, action.Destination.File.FullName, true);
                        else
                            DirectoryCopy(action.Source.Directory.FullName,
                                action.Destination.Directory.FullName, true, false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            foreach (var action in orderedDeletes)
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (action.Type)
                {
                    case ActionType.Delete:
                        if (!Options.EnableDeletes)
                            break;

                        if (action.Destination.IsFile)
                            File.Delete(action.Destination.File.FullName);
                        else
                            Directory.Delete(action.Destination.Directory.FullName, true);
                        break;
                }
        }

        private string GetDestinationFromSegment(FileInfoParserAction action)
        {
            var pp = new PathParser(Options.TargetDirectory);
            pp.AppendSegment(action.Source.Segment.GetSegmentString());
            var destinationSegment = pp.Segment.GetSegmentString();
            return destinationSegment;
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
                file.CopyTo(tempPath, true);
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