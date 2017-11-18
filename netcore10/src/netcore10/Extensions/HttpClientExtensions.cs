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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RedmineApi.Core.Extensions
{
    internal static class HttpClientExtensions
    {
        

        public static void EnsureValidHost(this string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new UriFormatException("Host is not define!");
            }

            if (!Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute))
            {
                throw new UriFormatException($"Host '{host}' is not valid!");
            }
        }

        public static void AddRequestHeader(this HttpClient httpClient, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            httpClient.ClearHeaderIfExists(key);
            httpClient.DefaultRequestHeaders.Add(key, value);
        }

        //public static void AddImpersonationHeaderIfSet(this HttpClient httpClient, string impersonateUser)
        //{
        //    if (string.IsNullOrWhiteSpace(impersonateUser))
        //    {
        //        httpClient.DefaultRequestHeaders.Remove(X_REDMINE_SWITCH_USER);
        //    }
        //    else
        //    {
        //        httpClient.ClearHeaderIfExists(X_REDMINE_SWITCH_USER);
        //        httpClient.DefaultRequestHeaders.Add(X_REDMINE_SWITCH_USER, impersonateUser);
        //    }
        //}

        //public static void AddApiKeyIfSet(this HttpClient httpClient, string apiKey)
        //{
        //    if (string.IsNullOrWhiteSpace(apiKey))
        //    {
        //        httpClient.DefaultRequestHeaders.Remove(X_REDMINE_API_KEY);
        //    }
        //    else
        //    {
        //        httpClient.ClearHeaderIfExists(X_REDMINE_API_KEY);
        //        httpClient.DefaultRequestHeaders.Add(X_REDMINE_API_KEY, apiKey);
        //    }
        //}

        public static void AddContentType(this HttpClient httpClient, string contentType)
        {
            var contentTypeKey = HttpRequestHeader.ContentType.ToString();
            httpClient.ClearHeaderIfExists(contentTypeKey);
            httpClient.DefaultRequestHeaders.Add(contentTypeKey, contentType);
        }

        private static void ClearHeaderIfExists(this HttpClient httpClient, string headerName)
        {
            if (httpClient.DefaultRequestHeaders.Any(h => h.Key == headerName))
            {
                httpClient.DefaultRequestHeaders.Remove(headerName);
            }
        }

        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(requestUri),
                Content = content,
            };

            return client.SendAsync(request);
        }
    }
}