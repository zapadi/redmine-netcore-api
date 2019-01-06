/*
   Copyright 2016 - 2017 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Diagnostics;

namespace RedmineApi.Core.IntegrationTests
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