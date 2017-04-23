using System;
using System.Diagnostics;
using Redmine.Net.Api;

namespace Tests
{
    public class RedmineManagerFixture : IDisposable
    {
        private readonly string host = "";
        private readonly string apiKey = "";

        public RedmineManagerFixture()
        {
            SetMimeTypeXml();
            SetMimeTypeJson();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJson()
        {
            RedmineManager = new RedmineManager(host, apiKey, MimeType.Json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXml()
        {
            RedmineManager = new RedmineManager(host, apiKey);
        }

        public void Dispose()
        {
            RedmineManager?.Dispose();
        }

        public RedmineManager RedmineManager { get; private set; }
    }
}