/*
   Copyright 2016 - 2019 Adrian Popescu.

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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace RedmineApi.Core.Extensions
{
    internal static class HttpClientExtensions
    {
        public static void AddRequestHeader(this HttpClient httpClient, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            httpClient.ClearHeaderIfExists(key);
            httpClient.DefaultRequestHeaders.Add(key, value);
        }

        public static void AddAcceptHeader(this HttpClient httpClient, string contentType)
        {
            httpClient.AddRequestHeader("Accept", contentType);
        }

        private static void ClearHeaderIfExists(this HttpClient httpClient, string headerName)
        {
            if (httpClient.DefaultRequestHeaders.Any(h => h.Key == headerName))
            {
                httpClient.DefaultRequestHeaders.Remove(headerName);
            }
        }

        public static void AddAuthentcation(this HttpClient httpClient, AuthenticationHeaderValue authenticationHeaderValue)
        {
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        }

        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"), RequestUri = new Uri(requestUri), Content = content,
            };

            return client.SendAsync(request, cancellationToken);
        }
    }
}