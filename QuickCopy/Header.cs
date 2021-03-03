using System;
using NLog;

namespace QuickCopy
{
    class Header
    {
        private const string HeaderString = @"
 _____                       __      ____                                
/\  __`\          __        /\ \    /\  _`\                              
\ \ \/\ \  __  __/\_\    ___\ \ \/'\\ \ \/\_\    ___   _____   __  __    
 \ \ \ \ \/\ \/\ \/\ \  /'___\ \ , < \ \ \/_/_  / __`\/\ '__`\/\ \/\ \   
  \ \ \\'\\ \ \_\ \ \ \/\ \__/\ \ \\`\\ \ \L\ \/\ \L\ \ \ \L\ \ \ \_\ \  
   \ \___\_\ \____/\ \_\ \____\\ \_\ \_\ \____/\ \____/\ \ ,__/\/`____ \ 
    \/__//_/\/___/  \/_/\/____/ \/_/\/_/\/___/  \/___/  \ \ \/  `/___/> \
                                                         \ \_\     /\___/
                                                          \/_/     \/__/ ";
        private const string Separator = @"----------------------------------------------------------------------";

        public void Print(ILogger logger, string programDirectory)
        {
            logger.Log(LogLevel.Info, HeaderString);
            logger.Log(LogLevel.Info, Separator);
            logger.Log(LogLevel.Info, "Corp: American College of Cardiology");
            logger.Log(LogLevel.Info, "Author: Jon Fast");
            logger.Log(LogLevel.Info, $"Version: {GetAssemblyVersion()}");
            logger.Log(LogLevel.Info,
                $"Current Run Time: {DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString()}");
            logger.Log(LogLevel.Info, $"Working Directory: {programDirectory}");
            logger.Log(LogLevel.Info, Separator + Environment.NewLine);
        }

        private string GetAssemblyVersion()
        {
            return GetType().Assembly.GetName().Version?.ToString();
        }
    }
}
