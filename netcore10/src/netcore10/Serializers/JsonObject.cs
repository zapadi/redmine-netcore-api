using System;
using Newtonsoft.Json;

namespace RedmineApi.Core.Serializers
{
    public class JsonObject : IDisposable
    {
        private static  JsonWriter Writer{ get; set; }
        private readonly bool hasRoot;

        public JsonObject(JsonWriter writer, string root = null)
        {
            Writer = writer;
            Writer.WriteStartObject();
            if (!string.IsNullOrWhiteSpace(root))
            {
                hasRoot = true;
                Writer.WritePropertyName(root);
                Writer.WriteStartObject();
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
