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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Types;

namespace RedmineApi.Core.Extensions
{
    internal static class HttpResponseMessageExtensions
    {
        internal static  Task<TaskCompletionSource<T>> CreateTaskCompletionSource<T>(this HttpResponseMessage responseMessage, MimeType mimeType) where T : class, new()
        {
            return  CreateTaskCompletionSource<T>(responseMessage, null, mimeType);
        }

        internal static async Task<TaskCompletionSource<T>> CreateTaskCompletionSource<T>(this HttpResponseMessage responseMessage, Func<string, T> func, MimeType mimeType) where T : class, new()
        {
            var tc = new TaskCompletionSource<T>();
            if (responseMessage.IsSuccessStatusCode)
            {
                var content = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                tc.SetResult(func != null ? func.Invoke(content) : RedmineSerializer.Deserialize<T>(content, mimeType));
            }
            else
            {
                tc.SetException(await CreateExceptionAsync(responseMessage, mimeType).ConfigureAwait(false));
            }

            return tc;
        }

        internal static async Task<TaskCompletionSource<byte[]>> CreateFileDownloadTaskCompletionSource(this HttpResponseMessage responseMessage, MimeType mimeType)
        {
            var tc = new TaskCompletionSource<byte[]>();
            if (responseMessage.IsSuccessStatusCode)
            {
                var response = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                tc.SetResult(response);
            }
            else
            {
                tc.SetException(await CreateExceptionAsync(responseMessage, mimeType).ConfigureAwait(false));
            }

            return tc;
        }

        internal static async Task<Exception> CreateExceptionAsync(this HttpResponseMessage responseMessage, MimeType mimeFormat)
        {
            var byteArray = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var statusCode = (int) responseMessage.StatusCode;
            string exceptionMessage;
            switch (statusCode)
            {
                case 422:
                    var errors = RedmineSerializer.DeserializeList<Error>(responseString, mimeFormat);

                    var message = string.Empty;
                    if (errors.Items != null)
                    {
                        message = errors.Items.Aggregate(message, (current, error) => $"{current}{error.Info}{Environment.NewLine}");
                    }

                    exceptionMessage = $"Request to {responseMessage.RequestMessage.RequestUri.AbsoluteUri} failed with {message}";
                    return new UnprocessableEntityException(exceptionMessage);

                default:
                    exceptionMessage = $"Request to {responseMessage.RequestMessage.RequestUri.AbsoluteUri} failed with status code {(int) responseMessage.StatusCode} ({responseMessage.ReasonPhrase}).";
                    return new RedmineException(exceptionMessage);
            }
        }
    }
}