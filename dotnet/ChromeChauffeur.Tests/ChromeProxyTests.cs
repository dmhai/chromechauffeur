using System;
using System.Threading;
using ChromeChauffeur.Core;
using ChromeChauffeur.Core.Settings;
using NUnit.Framework;

namespace ChromeChauffeur.Tests
{
    [TestFixture]
    public class ChromeProxyTests
    {
        private ChromeProxySettings _settings;
        private ChromeProxy _proxy;

        [SetUp]
        public void SetUp()
        {
            _settings = new ChromeProxySettings()
            {
                WindowState = WindowState.Minimized,
                DefaultTimeout = TimeSpan.FromSeconds(10)
            };

            _proxy = ChromeProxy.Launch(_settings);
        }

        [Test]
        public void Can_navigate_to_a_given_URL()
        {
            // This operation should execute and then wait until document is ready
            _proxy.GoToUrl("http://www.bing.com/");

            Assert.AreEqual("http://www.bing.com/", _proxy.GetCurrentUrl());

            _proxy.Write("Chrome Chauffeur!", ".b_searchbox");
            _proxy.Click(".b_searchboxSubmit");

            _proxy.WaitUntilElementExists(".sb_count");
        }

        [TearDown]
        public void TearDown()
        {
            _proxy.Dispose();
        }
    }
}
