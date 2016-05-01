using System;
using System.Diagnostics;
using System.Threading;
using ChromeChauffeur.Core.Chrome;
using ChromeChauffeur.Core.Chrome.InternalApi;
using ChromeChauffeur.Core.Exceptions;
using ChromeChauffeur.Core.Extensions;
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
            _remoteDebuggerClient.SendExpressionCommand($"document.location=\"{url}\"");

            WaitUntilDocumentIsReady(timeout);
        }

        public string GetCurrentUrl()
        {
            return Eval<string>("document.location.href");
        }

        public void Click(string cssSelector)
        {
            Click(cssSelector, _settings.DefaultTimeout);
        }

        public void Click(string cssSelector, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntilElementExists(cssSelector, timeout);

            Eval($"chromeChauffeur_privates.click(\"{cssSelector}\");", deadline);
        }

        public void WriteTo(string cssSelector, string text)
        {
            WriteTo(cssSelector, text, _settings.DefaultTimeout);
        }

        public void WriteTo(string cssSelector, string text, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntilElementExists(cssSelector, timeout);

            Eval($"chromeChauffeur_privates.write(\"{text}\", \"{cssSelector}\");", deadline);
        }

        public string GetInnerText(string selector)
        {
            return GetInnerText(selector, _settings.DefaultTimeout);
        }

        public string GetInnerText(string selector, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntilElementExists(selector, timeout);
            return Eval<string>($"document.querySelector(\"{selector}\").innerText", deadline);
        }

        public string GetInnerHtml(string selector)
        {
            return GetInnerHtml(selector, _settings.DefaultTimeout);
        }

        public string GetInnerHtml(string selector, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntilElementExists(selector, timeout);
            return Eval<string>($"document.querySelector(\"{selector}\").innerHTML", deadline);
        }

        public void SelectByValue(string selector, string value)
        {
            SelectByValue(selector, value, _settings.DefaultTimeout);
        }

        public void SelectByValue(string selector, string value, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntilElementExists(selector, timeout);
            Eval($"chromeChauffeur_privates.selectByValue(\"{selector}\", \"{value}\")", deadline);
        }

        public void SelectByIndex(string selector, int index)
        {
            SelectByIndex(selector, index, _settings.DefaultTimeout);
        }

        public void SelectByIndex(string selector, int index, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntilElementExists(selector, timeout);
            Eval($"chromeChauffeur_privates.selectByIndex(\"{selector}\", \"{index}\")", deadline);
        }

        public void SelectByText(string selector, string text)
        {
            SelectByText(selector, text, _settings.DefaultTimeout);
        }

        public void SelectByText(string selector, string text, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntilElementExists(selector, timeout);
            Eval($"chromeChauffeur_privates.selectByText(\"{selector}\", \"{text}\")", deadline);
        }

        public void WaitUntilElementExists(string selector)
        {
            WaitUntilElementExists(selector, _settings.DefaultTimeout);
        }

        public void WaitUntilElementExists(string selector, TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntil(
                () => Eval<bool>($"chromeChauffeur_privates.exists(\"{selector}\")", deadline), 
                $"Could not find element with selector \"{selector}\" within timeout", 
                timeout);
        }

        public void WaitUntilDocumentIsReady()
        {
            WaitUntilDocumentIsReady(_settings.DefaultTimeout);
        }

        public void WaitUntilDocumentIsReady(TimeSpan timeout)
        {
            var deadline = timeout.AsDeadline();

            WaitUntil(
                () => Eval<bool>("document.readyState === \"complete\"", deadline),
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

        private CommandResult Eval(string expression, DateTime deadline)
        {
            while (DateTime.Now < deadline)
            {
                EnsureJavaScriptHelpersInjected();

                var result = _remoteDebuggerClient.SendExpressionCommand(expression);

                if (!result.WasExceptionThrown)
                    return result;
            }

            throw new ChromeChauffeurTimeoutException($"Failed to execute command \"{expression}\" within timeout");
        }

        public string Eval(string expression)
        {
            var result = _remoteDebuggerClient.SendExpressionCommand(expression);

            if (result.WasExceptionThrown)
                throw new ChromeChauffeurException($"Command failed: {expression}");

            return result.Value;
        }

        public T Eval<T>(string expression, DateTime deadline)
        {
            var result = Eval(expression, deadline);
            return result.GetValue<T>();
        }

        public T Eval<T>(string expression)
        {
            return _remoteDebuggerClient.SendExpressionCommand(expression).GetValue<T>();
        }

        private void EnsureJavaScriptHelpersInjected()
        {
            var isDefined = Eval<bool>("typeof chromeChauffeur_privates !== 'undefined'");

            if (!isDefined)
            {
                InjectJavaScriptHelpers();
                InjectUserDefinedScripts();
                InjectOverrides();
            }
        }

        private void InjectJavaScriptHelpers()
        {
            var script = _embeddedResourceReader.Read("ChromeChauffeur.Core.Scripts.ChromeChauffeur.js", GetType().Assembly);
            Eval(script);
        }

        private void InjectUserDefinedScripts()
        {
            var customScript = _settings.CustomJavaScript;

            if (!string.IsNullOrWhiteSpace(customScript))
                Eval(customScript);
        }

        private void InjectOverrides()
        {
            if (_settings.BypassConfirmationPopup)
            {
                Eval("chromeChauffeur_privates.bypassConfirmationPopup(true)");
            }

            if (_settings.BypassAlertPopup)
            {
                Eval("chromeChauffeur_privates.bypassAlertPopup()");
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
