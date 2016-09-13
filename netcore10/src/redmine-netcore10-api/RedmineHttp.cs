using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Redmine.Net.Api.Internals;

namespace ClassLibrary
{
    public class RedmineHttp
    {
        private static readonly HttpClient httpClient; 

        static RedmineHttp()
        {
            httpClient = new HttpClient();
        }

        public static async Task<T> Get<T>(Uri uri, MimeType mimeType) where T: class, new() 
        {
            httpClient.BaseAddress = uri;
           var mes = new HttpRequestMessage();
            var response = await httpClient.SendAsync(mes);
            response.EnsureSuccessStatusCode();

          var resp =  await response.Content.ReadAsStringAsync();
          var x= RedmineSerializer.Deserialize<T>(resp, mimeType);

            var tc = new TaskCompletionSource<T>();
            tc.SetResult(x);
            return await tc.Task;
        }

        public static async Task<T> Put<T>(Uri uri)
        {
            var tc = new TaskCompletionSource<T>();
            tc.SetResult(default(T));
            return await tc.Task;
        }

        public static async Task<T> Post<T>(Uri uri)
        {
            var tc = new TaskCompletionSource<T>();
            tc.SetResult(default(T));
            return await tc.Task;
        }

        public static async Task<T> Delete<T>(Uri uri)
        {
            var tc = new TaskCompletionSource<T>();
            tc.SetResult(default(T));
            return await tc.Task;
        }

        public static void Dispose()
        {
            httpClient.Dispose();
        }
    }
}