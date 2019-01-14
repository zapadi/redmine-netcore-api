/*
   Copyright 2016 - 2019 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Serializers;

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
            var col = new List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonToken.StartArray)
                {
                    continue;
                }

                var obj = Activator.CreateInstance<T>();

                if (!(obj is IJsonSerializable ser))
                {
                    throw new RedmineException($"object '{typeof(T)}' should implement IJsonSerializable.");
                }

                ser.ReadJson(reader);

                var des = (T) ser;

                col.Add(des);
            }

            return col;
        }

        public static List<T> ReadAsArray<T>(this JsonReader reader) where T : class, new()
        {
            var col = new List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    break;
                }

                if (reader.TokenType == JsonToken.StartArray)
                {
                    continue;
                }

                var obj = Activator.CreateInstance<T>();

                if (!(obj is IJsonSerializable ser))
                {
                    throw new RedmineException($"object '{typeof(T)}' should implement IJsonSerializable.");
                }

                ser.ReadJson(reader);

                var des = (T) ser;

                col.Add(des);
            }

            return col;
        }
    }
}