using System;
using NLog;
using QuickCopy.Configuration;

namespace QuickCopy
{
    public static class ProgramExtensions
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void RunCopy(this ProgramOptions opts)
        {
            try
            {
                var changeDetector = new ChangeDetector(opts);
                if (!changeDetector.Changed())
                {
                    Log.Info("No changes detected");
                    return;
                }

                var copier = new DefaultCopier(opts);
                copier.Copy();
                Log.Info("Files copied");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void RunIncrementalCopy(this ProgramOptions opts)
        {
            try
            {
                var changeDetector = new ChangeDetector(opts);
                var changes = changeDetector.IncrementalChanges();
                var copier = new DefaultCopier(opts);
                copier.IncrementalCopy(changes);
                Log.Info("Files copied");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}