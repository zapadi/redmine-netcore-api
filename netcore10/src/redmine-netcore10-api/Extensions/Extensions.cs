using System;
using System.Net;
using System.Net.Http;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Extensions
{
    internal static class Extensions
    {
        public static void EnsureValidHost(this string host)
        {
            if (!Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute)) throw new UriFormatException($"Host '{host}' is not valid!");
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

        public static void AddContentType(this HttpClient httpClient, string contentType)
        {
            httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), contentType);
        }
    }

   
}