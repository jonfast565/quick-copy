using CommandLine;

namespace QuickCopy.OptionModels
{
    public class ProgramOptions
    {
        [Option('r', "runtime", Required = true, HelpText = "Sets the application runtime (console or service).")]
        public RuntimeType Runtime { get; set; }

        [Option('s', "sourceDirectory", Required = true, HelpText = "Source directory to watch for changes.")]
        public string SourceDirectory { get; set; }

        [Option('t', "targetDirectory", Required = true, HelpText = "Target directory.")]
        public string TargetDirectory { get; set; }

        [Option('c', "check", Required = false, HelpText = "Time interval (in ms) to check when there is new content.")]
        public double CheckTime { get; set; } = 3000D;

        [Option('d', "deletes", Required = false, Default = true, HelpText = "Enables/disables deletes in target (i.e. when the target has more items than the source)")]
        public bool EnableDeletes { get; set; }

        [Option('k', "skip", Required = false, HelpText = "The path fragment of some folder to skip")]
        public string SkipFolder { get; set; }
    }
}