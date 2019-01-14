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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Types;

namespace RedmineApi.Core
{
    internal sealed class RedmineHttpClient : IRedmineHttpClient
    {
        public MimeType MimeType { get; }
        private const string APPLICATION = "application";
        private const string X_REDMINE_SWITCH_USER = "X-Redmine-Switch-User";
        private const string X_REDMINE_API_KEY = "X-Redmine-API-Key";

        private static readonly Regex sanitizeRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private readonly HttpClientHandler clientHandler;

        private readonly HttpClient httpClient;
        private readonly string mediaType;

        public RedmineHttpClient(HttpClientHandler clientHandler, MimeType mimeType)
        {
            MimeType = mimeType;
            mediaType = $"{APPLICATION}/{Mime.GetStringRepresentation(mimeType)}";

            this.clientHandler = clientHandler;
            httpClient = new HttpClient(clientHandler, true);
        }

        public string ImpersonateUser { get; internal set; }

        public string ApiKey { get; internal set; }

        public void Dispose()
        {
            httpClient.CancelPendingRequests();
            clientHandler?.Dispose();
            httpClient.Dispose();
        }

        public async Task<T> GetAsync<T>(Uri uri,  CancellationToken cancellationToken) where T : class, new()
        {
            SetHeaders();

            httpClient.AddContentType(mediaType);

            using (var responseMessage = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource<T>(MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<PaginatedResult<T>> ListAsync<T>(Uri uri,  CancellationToken cancellationToken) where T : class, new()
        {
            SetHeaders();

            httpClient.AddContentType(mediaType);

            using (var responseMessage = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource(c => RedmineSerializer.DeserializeList<T>(c, MimeType), MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<T> PutAsync<T>(Uri uri, T data,  CancellationToken cancellationToken) where T : class, new()
        {
            SetHeaders();

            var serializedData = RedmineSerializer.Serialize(data, MimeType);
            serializedData = sanitizeRegex.Replace(serializedData, "\r\n");
            var requestContent = new StringContent(serializedData, Encoding.UTF8, mediaType);

            using (var responseMessage = await httpClient.PutAsync(uri.ToString(), requestContent, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource(c=>string.IsNullOrWhiteSpace(c)
                                                                                 ? data
                                                                                 : RedmineSerializer.Deserialize<T>(c, MimeType),MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<T> PostAsync<T>(Uri uri, T data, CancellationToken cancellationToken) where T : class, new()
        {
            SetHeaders();

            var content = new StringContent(RedmineSerializer.Serialize(data, MimeType), Encoding.UTF8, mediaType);

            using (var responseMessage = await httpClient.PostAsync(uri.ToString(), content, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource<T>(MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(Uri uri,  CancellationToken cancellationToken)
        {
            SetHeaders();

            using (var responseMessage = await httpClient.DeleteAsync(uri.ToString(), cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.DeleteTaskCompletionSource(MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<byte[]> DownloadFileAsync(Uri uri,  CancellationToken cancellationToken)
        {
            SetHeaders();

            using (var responseMessage = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.FileDownloadTaskCompletionSource(MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<Upload> UploadFileAsync(Uri uri, byte[] bytes,  CancellationToken cancellationToken)
        {
            SetHeaders();

            httpClient.AddContentType("application/octet-stream");

            var content = new ByteArrayContent(bytes);
            using (var responseMessage = await httpClient.PutAsync(uri.ToString(), content, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource<Upload>(MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<Upload> UploadAttachmentAsync(Uri uri, string attachmentContent, CancellationToken cancellationToken)
        {
            SetHeaders();

            var content = new StringContent(attachmentContent, Encoding.UTF8, mediaType);

            using (var responseMessage = await httpClient.PatchAsync(uri.ToString(), content, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource<Upload>(MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        public async Task<int> CountAsync<T>(Uri uri,  CancellationToken cancellationToken) where T : new()
        {
            SetHeaders();

            httpClient.AddContentType(mediaType);

            using (var responseMessage = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false))
            {
                var tc = await responseMessage.CreateTaskCompletionSource(c => RedmineSerializer.Count<T>(c, MimeType), MimeType).ConfigureAwait(false);
                return await tc.Task;
            }
        }

        private void SetHeaders()
        {
            httpClient.AddRequestHeader(X_REDMINE_API_KEY, ApiKey);
            httpClient.AddRequestHeader(X_REDMINE_SWITCH_USER, ImpersonateUser);
        }
    }
}