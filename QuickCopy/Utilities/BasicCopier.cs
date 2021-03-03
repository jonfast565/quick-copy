﻿using System;
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
                        var destinationSegment = action.GetDestinationFromSegment(Options.TargetDirectory);
                        if (action.Source.Segment.ContainsAllOfSegment(skipFolder.Segment))
                        {
                            Log.Info($"Skipped {action.Source.GetPath()} because {skipFolder.Segment.GetSegmentString()} skipped.");
                            break;
                        }

                        if (action.Source.IsFile)
                        {
                            File.Copy(action.Source.GetPath(), 
                                destinationSegment, true);
                            Log.Info($"Copied {action.Source.GetPath()}");
                        }
                        else
                            DirectoryCopy(action.Source.GetPath(), 
                                destinationSegment, true, false);
                        break;
                    case ActionType.Update:
                        if (action.Source.Segment.ContainsAllOfSegment(skipFolder.Segment))
                        {
                            Log.Info($"Skipped {action.Source.GetPath()} because {skipFolder.Segment.GetSegmentString()} skipped.");
                            break;
                        }

                        if (action.Source.IsFile)
                        {
                            File.Copy(action.Source.GetPath(), 
                                action.Destination.GetPath(), true);
                            Log.Info($"Copied {action.Source.GetPath()} (changed)");
                        }
                        else
                            DirectoryCopy(action.Source.GetPath(),
                                action.Destination.GetPath(), true, false);
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
                        {
                            Log.Info("Deletes suppressed by config");
                            break;
                        }

                        if (action.Destination.IsFile)
                            File.Delete(action.Destination.GetPath());
                        else
                            Directory.Delete(action.Destination.GetPath(), true);
                        break;
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