using Newtonsoft.Json;

namespace RedmineApi.Core.Serializers
{
    internal interface IJsonSerializable
    {
        void WriteJson(JsonWriter writer);
        void ReadJson(JsonReader reader);
    }
}