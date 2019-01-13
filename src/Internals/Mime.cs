using System.Collections.Generic;

namespace RedmineApi.Core.Internals
{
    public static class Mime
    {
        private static readonly Dictionary<MimeType, string> types = new Dictionary<MimeType, string>
        {
            [MimeType.Json] = MimeType.Json.ToString().ToLowerInvariant(),
            [MimeType.Xml] = MimeType.Xml.ToString().ToLowerInvariant()
        };

        public static string GetStringRepresentation(MimeType mimeType)
        {
            return types[mimeType];
        }
    }
}