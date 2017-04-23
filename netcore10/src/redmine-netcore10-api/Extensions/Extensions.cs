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
using System.Net;
using System.Net.Http;

namespace Redmine.Net.Api.Extensions
{
    internal static class Extensions
    {
        public static void EnsureValidHost(this string host)
        {
            if (!Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute))
                throw new UriFormatException($"Host '{host}' is not valid!");
        }

        public static void AddRequestHeader(this HttpClient httpClient, string key, string value)
        {
            httpClient.DefaultRequestHeaders.Add(key, value);
        }

        public static void AddImpersonationHeaderIfSet(this HttpClient httpClient, string impersonateUser)
        {
            if (string.IsNullOrWhiteSpace(impersonateUser))
                httpClient.DefaultRequestHeaders.Remove("X-Redmine-Switch-User");
            else
                httpClient.DefaultRequestHeaders.Add("X-Redmine-Switch-User", impersonateUser);
        }

        public static void AddApiKeyIfSet(this HttpClient httpClient, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                httpClient.DefaultRequestHeaders.Remove("X-Redmine-API-Key");
            else
                httpClient.DefaultRequestHeaders.Add("X-Redmine-API-Key", apiKey);
        }

        public static void AddContentType(this HttpClient httpClient, string contentType)
        {
            httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), contentType);
        }
    }
}