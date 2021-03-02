﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using QuickCopy.OptionModels;
using QuickCopy.PathModels;

namespace QuickCopy.Utilities
{
    public class ChangeDetector
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public ChangeDetector(ProgramOptions opts)
        {
            Options = opts;
        }

        public ProgramOptions Options { get; set; }

        public bool Changed()
        {
            Log.Info("Checking for changes...");
            var threeWayMerge = ThreeWayMerge();
            return threeWayMerge.Count > 0;
        }

        public List<FileInfoParserAction> IncrementalChanges()
        {
            Log.Info("Checking for changes...");
            var threeWayMerge = ThreeWayMerge();
            return threeWayMerge;
        }

        private List<FileInfoParserAction> ThreeWayMerge()
        {
            var dir1 = new DirectoryInfo(Options.SourceDirectory);
            var dir2 = new DirectoryInfo(Options.TargetDirectory);

            Log.Info($"Enumerate source directory: {dir1.FullName}");
            var files1 = new FileSystemEnumerable(dir1, "*.*", SearchOption.AllDirectories)
                .Select(x => new FileInfoParser(x.FullName, Options.SourceDirectory)).ToList();

            Log.Info($"Enumerate target directory: {dir2.FullName}");
            var files2 = new FileSystemEnumerable(dir2, "*.*", SearchOption.AllDirectories)
                .Select(x => new FileInfoParser(x.FullName, Options.TargetDirectory)).ToList();

            var inFirstOnly = new List<FileInfoParser>();
            var inBoth = new List<Tuple<FileInfoParser, FileInfoParser>>();

            Log.Info("Checking for created or updated files");
            foreach (var file1 in files1)
            {
                var foundInFirstOnly = true;
                foreach (var file2 in files2.Where(file2 =>
                    string.Equals(file1.PathSegmentHead.GetSegmentString(),
                        file2.PathSegmentHead.GetSegmentString(),
                        StringComparison.CurrentCultureIgnoreCase)))
                {
                    inBoth.Add(new Tuple<FileInfoParser, FileInfoParser>(file1, file2));
                    foundInFirstOnly = false;
                    break;
                }

                if (!foundInFirstOnly) continue;
                inFirstOnly.Add(file1);
            }

            Log.Info("Checking for deleted files");
            var inSecondOnly = new List<FileInfoParser>();
            foreach (var file2 in files2)
            {
                var foundInSecondOnly = files1.All(file1 => !string.Equals(file1.PathSegmentHead.GetSegmentString(),
                    file2.PathSegmentHead.GetSegmentString(), StringComparison.CurrentCultureIgnoreCase));
                if (foundInSecondOnly) inSecondOnly.Add(file2);
            }

            Log.Info("Enumerating possible actions");
            var actions = new List<FileInfoParserAction>();
            
            var firstPaths = inFirstOnly.Select(first => new FileInfoParserAction(first,
                    null,
                    ActionType.Create))
                .ToList();

            var secondPaths = inSecondOnly.Select(second => new FileInfoParserAction(
                null, second,
                ActionType.Delete));

            actions.AddRange(firstPaths);
            actions.AddRange(secondPaths);

            foreach (var (item1, item2) in inBoth)
            {
                if (!item1.IsFile || !item2.IsFile) continue;

                if (item1.File.Length != item2.File.Length)
                    actions.Add(new FileInfoParserAction(item1, item2, ActionType.Update));
            }

            return actions;
        }
    }
}