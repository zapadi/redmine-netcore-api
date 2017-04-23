using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using System.Linq;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api
{
    internal class RedmineHttp
    {
        private static readonly HttpClient httpClient;

        static RedmineHttp()
        {
            //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            //  httpClient.DefaultRequestHeaders.ProxyAuthorization = new AuthenticationHeaderValue("","");

            //HttpClientHandler handler = new HttpClientHandler()
            //{
            // //   Proxy = Proxy,
            // //   UseProxy = true,
            //    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None,
            // //   CookieContainer = new CookieContainer(),
            // //   PreAuthenticate = true,
            // //   SslProtocols = SslProtocols.Tls12,

            //};

            httpClient = new HttpClient();
        }

        public static IWebProxy Proxy { get; private set; }

        public static string ImpersonateUser { get; set; }

        public static async Task<T> Get<T>(Uri uri, MimeType mimeType) where T : class, new()
        {
            httpClient.AddContentType($"application/{UrlBuilder.mimeRepresentation[mimeType]}");
            var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false);
            var tc = await CreateTaskCompletionSource<T>(responseMessage, mimeType).ConfigureAwait(false);
            return await tc.Task;
        }

        public static async Task<PaginatedResult<T>> List<T>(Uri uri, MimeType mimeType) where T : class, new()
        {
            httpClient.AddContentType($"application/{UrlBuilder.mimeRepresentation[mimeType]}");

            var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false);

            var tc = await CreateTaskCompletionSource(responseMessage, c => RedmineSerializer.DeserializeList<T>(c, mimeType), mimeType).ConfigureAwait(false);
            return await tc.Task;
        }

        public static async Task<T> Put<T>(Uri uri, T data, MimeType mimeType) where T : class, new()
        {
            var serializedData = RedmineSerializer.Serialize(data, mimeType);
            serializedData = Regex.Replace(serializedData, @"\r\n|\r|\n", "\r\n");
            var requestContent = new StringContent(serializedData, Encoding.UTF8, $"application/{UrlBuilder.mimeRepresentation[mimeType]}");

            var responseMessage = await httpClient.PutAsync(uri.ToString(), requestContent).ConfigureAwait(false);
            var tc = new TaskCompletionSource<T>();
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    tc.SetResult(data);
                }
                else
                {
                    tc.SetResult(RedmineSerializer.Deserialize<T>(responseContent, mimeType));
                }
            }
            else
            {
                tc.SetException(await CreateExceptionAsync(responseMessage, mimeType));
            }
            return await tc.Task;
        }

        private static async Task<TaskCompletionSource<T>> CreateTaskCompletionSource<T>(HttpResponseMessage responseMessage, MimeType mimeType) where T : class, new()
        {
            return await CreateTaskCompletionSource<T>(responseMessage, null, mimeType);
        }

        private static async Task<TaskCompletionSource<T>> CreateTaskCompletionSource<T>(HttpResponseMessage responseMessage, Func<string, T> func, MimeType mimeType) where T : class, new()
        {
            var tc = new TaskCompletionSource<T>();
            if (responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                tc.SetResult(func != null ? func.Invoke(content) : RedmineSerializer.Deserialize<T>(content, mimeType));
            }
            else
            {
                tc.SetException(await CreateExceptionAsync(responseMessage, mimeType));
            }

            return tc;
        }

        private static async Task<TaskCompletionSource<byte[]>> CreateFileDownloadTaskCompletionSource(HttpResponseMessage responseMessage, MimeType mimeType)
        {
            var tc = new TaskCompletionSource<byte[]>();
            if (responseMessage.IsSuccessStatusCode)
            {
                byte[] response = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                tc.SetResult(response);
            }
            else
            {
                tc.SetException(await CreateExceptionAsync(responseMessage, mimeType));
            }

            return tc;
        }

        public static async Task<T> Post<T>(Uri uri, T data, MimeType mimeType) where T : class, new()
        {
            var content = new StringContent(RedmineSerializer.Serialize(data, mimeType), Encoding.UTF8, $"application/{UrlBuilder.mimeRepresentation[mimeType]}");
            var responseMessage = await httpClient.PostAsync(uri.ToString(), content).ConfigureAwait(false);

            var tc = await CreateTaskCompletionSource<T>(responseMessage, mimeType).ConfigureAwait(false);
            return await tc.Task;
        }

        public static async Task<HttpStatusCode> Delete(Uri uri, MimeType mimeType)
        {
            var tc = new TaskCompletionSource<HttpStatusCode>();
            try
            {
                var responseMessage = await httpClient.DeleteAsync(uri.ToString()).ConfigureAwait(false);

                if (responseMessage.IsSuccessStatusCode)
                {
                    tc.SetResult(responseMessage.StatusCode);
                }
                else
                {
                    tc.SetException(await CreateExceptionAsync(responseMessage, mimeType));
                }
            }
            catch (Exception exception)
            {
                tc.SetException(exception);
            }
            return await tc.Task;
        }

        public static async Task<byte[]> DownloadFile(Uri uri, MimeType mimeType)
        {
            var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false);
            var tc = await CreateFileDownloadTaskCompletionSource(responseMessage, mimeType).ConfigureAwait(false);
            return await tc.Task;
        }

        public static async Task<Upload> UploadFile(Uri uri, byte[] bytes, MimeType mimeType)
        {
            var content = new ByteArrayContent(bytes);
            httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/octet-stream");
            var responseMessage = await httpClient.PutAsync(uri.ToString(), content).ConfigureAwait(false);

            var tc = await CreateTaskCompletionSource<Upload>(responseMessage, mimeType).ConfigureAwait(false);
            return await tc.Task;
        }

        public static void Dispose()
        {
            httpClient.CancelPendingRequests();
            httpClient.Dispose();
        }

        private static async Task<Exception> CreateExceptionAsync(HttpResponseMessage responseMessage, MimeType mimeFormat)
        {
            var byteArray = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            var statusCode = (int)responseMessage.StatusCode;
            switch (statusCode)
            {
                case 422:
                    var errors = RedmineSerializer.DeserializeList<Error>(responseString, mimeFormat);

                    var message = string.Empty;
                    if (errors.Items != null)
                    {
                        message = errors.Items.Aggregate(message, (current, error) => $"{current}{error.Info}{Environment.NewLine}");
                    }
                    return new UnprocessableEntityException($"Request to {responseMessage.RequestMessage.RequestUri.AbsoluteUri} failed with {message}");

                default:
                    var exceptionMessage =
                        $"Request to {responseMessage.RequestMessage.RequestUri.AbsoluteUri} failed with status code {(int)responseMessage.StatusCode} ({responseMessage.ReasonPhrase}).";

                    return new RedmineException(exceptionMessage);
            }
        }
    }
}