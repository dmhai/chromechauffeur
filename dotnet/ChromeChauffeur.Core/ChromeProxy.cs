using System;
using System.Diagnostics;
using System.Threading;
using ChromeChauffeur.Core.Chrome;
using ChromeChauffeur.Core.Exceptions;
using ChromeChauffeur.Core.Infrastructure.IO;
using ChromeChauffeur.Core.Infrastructure.Processes;
using ChromeChauffeur.Core.Settings;

namespace ChromeChauffeur.Core
{
    public class ChromeProxy : IDisposable
    {
        private readonly RemoteDebuggerClient _remoteDebuggerClient;
        private readonly EmbeddedResourceReader _embeddedResourceReader;
        private readonly ChromeProcess _process;
        private readonly ChromeProxySettings _settings;

        private ChromeProxy(ChromeProcess process, ChromeProxySettings settings)
        {
            _process = process;
            _settings = settings;
            _remoteDebuggerClient = new RemoteDebuggerClient();
            _embeddedResourceReader = new EmbeddedResourceReader();
        }

        private void Attach(ChromeProxySettings settings)
        {
            _remoteDebuggerClient.Connect("localhost", settings.PortNumber);
        }

        public void GoToUrl(string url)
        {
            GoToUrl(url, _settings.DefaultTimeout);
        }

        public void GoToUrl(string url, TimeSpan timeout)
        {
            _remoteDebuggerClient.SendExpressionCommand($"document.location='{url}'");

            WaitUntilDocumentIsReady(timeout);
        }

        public string GetCurrentUrl()
        {
            return _remoteDebuggerClient.SendExpressionCommand("document.location.href");
        }

        public void Click(string cssSelector)
        {
            EnsureJavaScriptHelpersInjected();
            Eval($"chromeChauffeur_privates.click('{cssSelector}');");
        }

        public void Write(string text, string cssSelector)
        {
            EnsureJavaScriptHelpersInjected();
            Eval($"chromeChauffeur_privates.write('{text}', '{cssSelector}');");
        }

        public void WaitUntilElementExists(string selector)
        {
            WaitUntilElementExists(selector, _settings.DefaultTimeout);
        }

        public void WaitUntilElementExists(string selector, TimeSpan timeout)
        {
            WaitUntil(
                () => Eval<bool>($"chromeChauffeur_privates.exists('{selector}')"), 
                $"Could not find element with selector '{selector}' within timeout", 
                timeout);
        }

        public void WaitUntilDocumentIsReady()
        {
            WaitUntilDocumentIsReady(_settings.DefaultTimeout);
        }

        public void WaitUntilDocumentIsReady(TimeSpan timeout)
        {
            WaitUntil(
                () => Eval<bool>("document.readyState === 'complete'"),
                "Document was not ready within timeout",
                timeout);
        }

        public void WaitUntil(Func<bool> predicate, string timeoutMessage, TimeSpan timeout)
        {
            var deadline = DateTime.Now.Add(timeout);

            WaitUntil(predicate, timeoutMessage, deadline);
        }

        public void WaitUntil(Func<bool> predicate, string timeoutMessage, DateTime deadline)
        {
            while (DateTime.Now < deadline)
            {
                EnsureJavaScriptHelpersInjected();

                if (predicate())
                    return;

                Thread.Sleep(100);
            }

            throw new ChromeChauffeurTimeoutException(timeoutMessage);
        }

        public string Eval(string expression)
        {
            return _remoteDebuggerClient.SendExpressionCommand(expression);
        }

        public T Eval<T>(string expression)
        {
            var result = _remoteDebuggerClient.SendExpressionCommand(expression);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        private void EnsureJavaScriptHelpersInjected()
        {
            var isDefined = Eval<bool>("typeof chromeChauffeur_privates !== 'undefined'");

            if (!isDefined)
            {
                var script = _embeddedResourceReader.Read("ChromeChauffeur.Core.Scripts.ChromeChauffeur.js", GetType().Assembly);
                Eval(script);
            }
        }

        public static ChromeProxy Launch(ChromeProxySettings settings = null)
        {
            settings = settings ?? new ChromeProxySettings();

            var process = ChromeProcess.Start(settings);
            var chromeProxy = new ChromeProxy(process, settings);

            chromeProxy.Attach(settings);

            return chromeProxy;
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }
}
