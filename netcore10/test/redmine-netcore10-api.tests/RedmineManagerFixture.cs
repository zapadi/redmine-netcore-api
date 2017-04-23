using System;
using System.Diagnostics;
using Redmine.Net.Api;

namespace Tests
{
    public class RedmineManagerFixture : IDisposable
    {
        private readonly string host = "";

        public RedmineManagerFixture()
        {
            IAuthentication authentication = StatelessAuthentication.Create("");

            //authentication = BasicAuthentication.Create("", "");

            SetMimeTypeXml(authentication);
            SetMimeTypeJson(authentication);
        }

        [Conditional("JSON")]
        private void SetMimeTypeJson(IAuthentication authentication)
        {
            RedmineManager = new RedmineManager(host, authentication, MimeType.Json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXml(IAuthentication authentication)
        {
            RedmineManager = new RedmineManager(host, authentication);
        }

        public void Dispose()
        {
            RedmineManager?.Dispose();
        }

        public RedmineManager RedmineManager { get; private set; }
    }
}