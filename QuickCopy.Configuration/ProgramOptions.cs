using System.Collections.Generic;
using CommandLine;

namespace QuickCopy.Configuration
{
    public class ProgramOptions
    {
        [Option('r', "runtime", Required = false, HelpText = "Sets the application runtime (console or service).")]
        public RuntimeType Runtime { get; set; }

        [Option('s', "sourceDirectory", Required = false, HelpText = "Source directory to watch for changes.")]
        public string SourceDirectory { get; set; }

        [Option('t', "targetDirectories", Required = false, HelpText = "Target directory.")]
        public IList<string> TargetDirectories { get; set; }

        [Option('c', "check", Required = false, HelpText = "Time interval (in ms) to check when there is new content.")]
        public double CheckTime { get; set; } = 30000D;

        [Option('d', "deletes", Required = false, Default = true, HelpText = "Enables/disables deletes in target (i.e. when the target has more items than the source)")]
        public bool EnableDeletes { get; set; }

        [Option('k', "skipFolders", Required = false, Separator = ',', HelpText = "The path fragment(s) of some folder(s) to skip")]
        public IList<string> SkipFolders { get; set; }

        [Option('g', "config", Required = false, Default = false, HelpText = "Determines whether a config file will be used in this case")]
        public bool UseConfigFile { get; set; }
    }
}