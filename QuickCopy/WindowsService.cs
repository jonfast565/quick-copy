using System.ServiceProcess;
using System.Timers;
using QuickCopy.OptionModels;
using QuickCopy.Utilities;

namespace QuickCopy
{
    public class WindowsService : ServiceBase
    {
        private readonly ProgramOptions _options;
        private Timer _timer;

        public WindowsService(ProgramOptions options)
        {
            _options = options;
            ServiceName = "QuickCopy";
        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer(_options.CheckTime)
            {
                AutoReset = true
            };
            _timer.Elapsed += Elapsed;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer = null;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            _options.RunCopy();
        }
    }
}