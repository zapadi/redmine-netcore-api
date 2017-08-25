using Newtonsoft.Json;
using System;

namespace RedmineApi.Core.Extensions
{
    public class JsonObject : IDisposable
    {
        private static  JsonWriter Writer{ get; set; }
        private bool hasRoot;

        public JsonObject(JsonWriter writer, string root = null)
        {
            Writer = writer;
            Writer.WriteStartObject();
            if (string.IsNullOrWhiteSpace(root))
            {
                hasRoot = true;
                Writer.WritePropertyName(root);
            }
        }

        public void Dispose()
        {
            Writer.WriteEndObject();
            if(hasRoot)
            {
                Writer.WriteEndObject();
            }
        }
    }
}
