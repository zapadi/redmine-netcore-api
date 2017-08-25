using Newtonsoft.Json;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Serializers;
using System;
using System.Collections.Generic;

namespace RedmineApi.Core.Extensions
{
    public static partial class JsonExtensions
    {

        public static int ReadAsInt(this JsonReader reader)
        {
            return reader.ReadAsInt32().GetValueOrDefault();
        }
        public static bool ReadAsBool(this JsonReader reader)
        {
            return reader.ReadAsBoolean().GetValueOrDefault();
        }

        public static List<T> ReadAsCollection<T>(this JsonReader reader) where T : class, new()
        {
            List<T> col = new List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    continue;
                }

                if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

                var obj = Activator.CreateInstance<T>();
                var ser = obj as IJsonSerializable;

                if (ser == null)
                {
                    throw new RedmineException($"object '{typeof(T)}' should implement IJsonSerializable.");
                }

                ser.ReadJson(reader);

                T des = (T)ser;
                if (des != default(T))
                {
                    col.Add(des);
                }
            }

            return col;
        }
    }
}
