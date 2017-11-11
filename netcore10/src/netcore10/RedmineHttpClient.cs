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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Types;

namespace RedmineApi.Core
{
    internal sealed class RedmineHttpClient : IDisposable
    {
        private const string APPLICATION = "application";

        private readonly HttpClient httpClient;
        private readonly HttpClientHandler clientHandler;

        private static readonly Regex sanitizeRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled | RegexOptions.CultureInvariant);


        public RedmineHttpClient(HttpClientHandler clientHandler)
        {
            this.clientHandler = clientHandler;
            httpClient = new HttpClient(clientHandler, true);
        }

        public string ImpersonateUser { get; set; }

        public string ApiKey { get; set; }

        public void Dispose()
        {
            httpClient.CancelPendingRequests();
            clientHandler.Dispose();
            httpClient.Dispose();
        }

        public async Task<T> Get<T>(Uri uri, MimeType mimeType) where T : class, new()
        {
            httpClient.AddApiKeyIfSet(ApiKey);
            httpClient.AddImpersonationHeaderIfSet(ImpersonateUser);
            httpClient.AddContentType($"{APPLICATION}/{UrlBuilder.MimeTypes[mimeType]}");

            using (var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource<T>(mimeType).ConfigureAwait(false);

                return await tc.Task;
            }
        }

        public async Task<PaginatedResult<T>> List<T>(Uri uri, MimeType mimeType) where T : class, new()
        {
            httpClient.AddApiKeyIfSet(ApiKey);
            httpClient.AddImpersonationHeaderIfSet(ImpersonateUser);
            httpClient.AddContentType($"{APPLICATION}/{UrlBuilder.MimeTypes[mimeType]}");

            using (var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource(c => RedmineSerializer.DeserializeList<T>(c, mimeType), mimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<T> Put<T>(Uri uri, T data, MimeType mimeType) where T : class, new()
        {
            httpClient.AddApiKeyIfSet(ApiKey);
            httpClient.AddImpersonationHeaderIfSet(ImpersonateUser);

            var serializedData = RedmineSerializer.Serialize(data, mimeType);
            serializedData = sanitizeRegex.Replace(serializedData, "\r\n");
            var requestContent = new StringContent(serializedData, Encoding.UTF8, $"{APPLICATION}/{UrlBuilder.MimeTypes[mimeType]}");

            using (var responseMessage = await httpClient.PutAsync(uri.ToString(), requestContent).ConfigureAwait(false))
            {
                var tc = new TaskCompletionSource<T>();
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    tc.SetResult(string.IsNullOrWhiteSpace(responseContent)
                        ? data
                        : RedmineSerializer.Deserialize<T>(responseContent, mimeType));
                }
                else
                {
                    tc.SetException(await responseMessage.CreateExceptionAsync(mimeType).ConfigureAwait(false));
                }
                return await tc.Task;
            }
        }
      
        public async Task<T> Post<T>(Uri uri, T data, MimeType mimeType) where T : class, new()
        {
            httpClient.AddApiKeyIfSet(ApiKey);
            httpClient.AddImpersonationHeaderIfSet(ImpersonateUser);

            var content = new StringContent(RedmineSerializer.Serialize(data, mimeType), Encoding.UTF8,
                $"{APPLICATION}/{UrlBuilder.MimeTypes[mimeType]}");

            using (var responseMessage = await httpClient.PostAsync(uri.ToString(), content).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource<T>(mimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<HttpStatusCode> Delete(Uri uri, MimeType mimeType)
        {
            var tc = new TaskCompletionSource<HttpStatusCode>();
            try
            {
                httpClient.AddApiKeyIfSet(ApiKey);
                httpClient.AddImpersonationHeaderIfSet(ImpersonateUser);

                using (var responseMessage = await httpClient.DeleteAsync(uri.ToString()).ConfigureAwait(false))
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        tc.SetResult(responseMessage.StatusCode);
                    }
                    else
                    {
                        tc.SetException(await responseMessage.CreateExceptionAsync(mimeType).ConfigureAwait(false));
                    }
                }
            }
            catch (Exception exception)
            {
                tc.SetException(exception);
            }
            return await tc.Task;
        }

        public async Task<byte[]> DownloadFile(Uri uri, MimeType mimeType)
        {
            httpClient.AddApiKeyIfSet(ApiKey);
            httpClient.AddImpersonationHeaderIfSet(ImpersonateUser);

            using (var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateFileDownloadTaskCompletionSource(mimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<Upload> UploadFile(Uri uri, byte[] bytes, MimeType mimeType)
        {
            httpClient.AddApiKeyIfSet(ApiKey);
            httpClient.AddImpersonationHeaderIfSet(ImpersonateUser);
            httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/octet-stream");

            var content = new ByteArrayContent(bytes);
            using (var responseMessage = await httpClient.PutAsync(uri.ToString(), content).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource<Upload>(mimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }
    }
}