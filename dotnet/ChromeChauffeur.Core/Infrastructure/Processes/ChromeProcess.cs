using System;
using System.Diagnostics;
using ChromeChauffeur.Core.Settings;

namespace ChromeChauffeur.Core.Infrastructure.Processes
{
    public class ChromeProcess : IDisposable
    {
        private readonly Process _process;

        private ChromeProcess(Process process)
        {
            _process = process;
        }

        public void Dispose()
        {
            if (!_process.HasExited)
                _process.Kill();

            _process?.Dispose();
        }

        public static ChromeProcess Start(ChromeProxySettings settings)
        {
            var process = CreateChromeProcess(settings);
            return new ChromeProcess(process);
        }

        private static Process CreateChromeProcess(ChromeProxySettings settings)
        {
            var args = $"--remote-debugging-port={settings.PortNumber} --user-data-dir={settings.ProfileDirectory} --no-first-run --no-default-browser-check";

            var processStartInfo = new ProcessStartInfo(settings.ChromeExecutablePath, args)
            {
                WindowStyle = settings.WindowState.GetProcessWindowStyle()
            };

            return Process.Start(processStartInfo);
        }
    }
}