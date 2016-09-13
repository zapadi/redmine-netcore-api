using System;

namespace ClassLibrary
{
    internal static class Extensions
    {
        public static void CheckIfHostIsValid(this string host)
        {
            if (!Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute)) throw new UriFormatException($"Host '{host}' is not valid!");
        }
    }
}