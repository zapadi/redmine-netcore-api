using Newtonsoft.Json;

namespace RedmineApi.Core.Serializers
{
    internal interface IJsonSerializable
    {
        void ReadJson(JsonWriter writer);
        void WriteJson(JsonReader reader);
    }
}