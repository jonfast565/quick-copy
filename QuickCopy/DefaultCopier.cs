using System;
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

        public void IncrementalCopy(List<FileInfoParserAction> actions)
        {
            var skipFolders = Options.SkipFolders
                .Select(x => new PathParser(x))
                .ToList();

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

                        foreach (var skipFolder in skipFolders.Where(skipFolder => 
                            action.Source.Segment.ContainsAllOfSegment(skipFolder.Segment)))
                        {
                            Log.Info(
                                $"Skipped {action.Source.GetPath()} because {skipFolder.Segment.GetSegmentString()} skipped.");
                            break;
                        }

                        if (action.Source.IsFile)
                        {
                            File.Copy(action.Source.GetPath(), 
                                destinationSegment, true);
                            Log.Info($"Copied {action.Source.GetPath()}");
                        }
                        else
                        {
                            Directory.CreateDirectory(action.Destination.GetPath());
                        }

                        break;
                    case ActionType.Update:
                        foreach (var skipFolder in skipFolders
                            .Where(skipFolder => action.Source.Segment.ContainsAllOfSegment(skipFolder.Segment)))
                        {
                            Log.Info(
                                $"Skipped {action.Source.GetPath()} because {skipFolder.Segment.GetSegmentString()} skipped.");
                            break;
                        }

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
    }
}