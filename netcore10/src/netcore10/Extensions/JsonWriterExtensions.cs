using Newtonsoft.Json;
using RedmineApi.Core.Types;
using System.Collections;

namespace RedmineApi.Core.Extensions
{
    public static partial class JsonExtensions
    {
        public static void WriteIdIfNotNull(this JsonWriter jsonWriter, string tag, IdentifiableName value)
        {
            if(value != null)
            {
                jsonWriter.WritePropertyName(tag);
                jsonWriter.WriteValue(value.Id);
            }
        }

        public static void WriteProperty(this JsonWriter jsonWriter, string tag, object value)
        {
            jsonWriter.WritePropertyName(tag);
            jsonWriter.WriteValue(value);
        }

        public static void WriteArrayIds(this JsonWriter jsonWriter, string tag, ICollection collection)
        {

        }
    }
}
