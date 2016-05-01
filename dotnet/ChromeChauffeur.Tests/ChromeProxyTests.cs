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
                WindowState = WindowState.Maximized,
                DefaultTimeout = TimeSpan.FromSeconds(10)
            };

            _proxy = ChromeProxy.Launch(_settings);
        }

        [Test]
        public void Can_search_bing()
        {
            // This operation should execute and then wait until document is ready
            _proxy.GoToUrl("http://www.bing.com/");

            Assert.AreEqual("http://www.bing.com/", _proxy.GetCurrentUrl());

            _proxy.WriteTo(".b_searchbox", "Chrome Chauffeur!");
            _proxy.Click(".b_searchboxSubmit");

            _proxy.WaitUntilElementExists(".sb_count");
        }

        [Test]
        public void Can_update_angular_js_field()
        {
            var title = Guid.NewGuid().ToString();

            _proxy.GoToUrl("http://localhost/cctestapp/angular");
            _proxy.Click("#add-item-link");
            _proxy.WriteTo("#title", title);
            _proxy.Click("#submit");

            _proxy.WaitUntilElementExists("#item-list");

            Assert.AreEqual(title, _proxy.GetInnerText("li:last-of-type"));
        }

        [TearDown]
        public void TearDown()
        {
            _proxy.Dispose();
        }
    }
}
