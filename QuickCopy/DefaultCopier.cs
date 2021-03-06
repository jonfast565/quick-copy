﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using QuickCopy.Configuration;
using QuickCopy.PathLib;

namespace QuickCopy
{
    public class DefaultCopier
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public DefaultCopier(ProgramOptions opts)
        {
            Options = opts;
        }

        public ProgramOptions Options { get; set; }

        public void Copy()
        {
            foreach (var targetDirectory in Options.TargetDirectories)
            {
                DirectoryCopy(Options.SourceDirectory, targetDirectory, true, true);
            }
        }

        public void IncrementalCopy(List<FileInfoParserActionList> actions)
        {
            foreach (var action in actions)
            {
                IncrementalCopyOne(action);
            }
        }

        private void IncrementalCopyOne(FileInfoParserActionList actionList)
        {

            var orderedCreates =
                actionList.Actions
                    .Where(x => x.Type == ActionType.Create || x.Type == ActionType.Update)
                    .OrderBy(x => x.GetSourceLength());

            var orderedDeletes =
                actionList.Actions
                    .Where(x => x.Type == ActionType.Delete)
                    .OrderByDescending(x => x.GetDestinationLength());

            foreach (var action in orderedCreates)
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (action.Type)
                {
                    case ActionType.Create:
                        var destinationSegment = action.GetDestinationFromSegment(actionList.TargetDirectory);
                        if (action.Source.IsFile)
                        {
                            File.Copy(action.Source.GetPath(),
                                destinationSegment, true);
                            Log.Info($"Copied {action.Source.GetPath()}");
                        }
                        else
                        {
                            Directory.CreateDirectory(destinationSegment);
                        }

                        break;
                    case ActionType.Update:
                        if (action.Source.IsFile)
                        {
                            File.Copy(action.Source.GetPath(),
                                action.Destination.GetPath(), true);
                            Log.Info($"Copied {action.Source.GetPath()} (changed)");
                        }
                        else
                        {
                            Directory.CreateDirectory(action.Destination.GetPath());
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            if (!Options.EnableDeletes)
            {
                Log.Info("Deletes suppressed by config");
            }
            else
            {
                foreach (var action in orderedDeletes)
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (action.Type)
                    {
                        case ActionType.Delete:
                            if (action.Destination.IsFile)
                                File.Delete(action.Destination.GetPath());
                            else
                                Directory.Delete(action.Destination.GetPath(), true);
                            break;
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