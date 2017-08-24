using Newtonsoft.Json;

namespace RedmineApi.Core.Serializers
{
    internal interface IJsonSerializable
    {
        void Serialize(JsonWriter writer);
        void Deserialize(JsonReader reader);
    }
}