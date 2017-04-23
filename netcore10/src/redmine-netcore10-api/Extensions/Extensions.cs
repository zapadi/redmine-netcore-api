using System;

namespace Redmine.Net.Api.Extensions
{
    internal static class Extensions
    {
        public static void EnsureValidHost(this string host)
        {
            if (!Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute)) throw new UriFormatException($"Host '{host}' is not valid!");
        }
    }
}