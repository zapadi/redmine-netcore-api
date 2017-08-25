using Newtonsoft.Json;

namespace RedmineApi.Core.Serializers
{
    public interface IJsonSerializable
    {
        void WriteJson(JsonWriter writer);
        void ReadJson(JsonReader reader);
    }
}