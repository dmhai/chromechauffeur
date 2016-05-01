using System;
using System.IO;

namespace ChromeChauffeur.Core.Settings
{
    public class ChromeProxySettings
    {
        public ChromeProxySettings()
        {
            var random = new Random();

            ChromeExecutablePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            ProfileDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()).Replace(" ", @"\\ ");
            PortNumber = random.Next(16000, 20000);
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// The port number to use for the communication with the Chrome instance.
        /// By default, a random port in the range 16000-20000 is used.
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        /// The path to the actual chrome.exe file used to launch Chrome.
        /// </summary>
        public string ChromeExecutablePath { get; set; }

        /// <summary>
        /// Path to the profile directory to use for the chrome session.
        /// By default, a temp directory is created and used for the session profile data.
        /// Make sure to escape spaces with a double slash, i.e. "Program Files" => "Program\\ Files"
        /// </summary>
        public string ProfileDirectory { get; set; }

        /// <summary>
        /// Specifies whether to launch Chrome with the window minimized, maximized or neither
        /// </summary>
        public WindowState WindowState { get; set; }

        /// <summary>
        /// The default timeout used when executing commands in the browser or waiting for elements to be rendered
        /// </summary>
        public TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// Custom JavaScript code that should be injected into the pages of the Chrome instance. 
        /// This can be used to simplify the test code by adding JavaScript helper functions that
        /// can be called from the tests via an Eval call.
        /// </summary>
        public string CustomJavaScript { get; set; }

        /// <summary>
        /// Configures the driver to replace the default window.confirm behaviour
        /// to simply return true instead of showing the actual confirmation popup.
        /// </summary>
        public bool BypassConfirmationPopup { get; set; }

        /// <summary>
        /// Configures the driver to replace the default window.alert behaviour
        /// to do nothing instead of showing the actual confirmation popup.
        /// </summary>
        public bool BypassAlertPopup { get; set; }
    }
}