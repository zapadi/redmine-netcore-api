/*
   Copyright 2016 - 2017 Adrian Popescu.

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
using RedmineApi.Core.Types;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Extensions;

namespace RedmineApi.Core.Internals
{
    internal sealed class RedmineJsonSerializer : IRedmineSerializer
    {
        public T Deserialize<T>(string response) where T : class
        {
            using (var sr = new StringReader(response))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var obj = Activator.CreateInstance<T>();
                    var ser = obj as IJsonSerializable;

                    if (ser == null)
                    {
                        throw new RedmineException($"object '{typeof(T)}' should implement IJsonSerializable.");
                    }

                    if (reader.Read())
                    {
                        reader.Read();
                        ser.WriteJson(reader);
                    }
                    return (T)ser;
                }
            }
        }

        public PaginatedResult<T> DeserializeList<T>(string response) where T : class, new()
        {
            using (var sr = new StringReader(response))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    int total = 0;
                    int offset = 0;
                    int limit = 0;
                    List<T> list = null;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            switch (reader.Value)
                            {
                                case RedmineKeys.TOTAL_COUNT: total = reader.ReadAsInt32().GetValueOrDefault(); break;
                                case RedmineKeys.OFFSET: offset = reader.ReadAsInt32().GetValueOrDefault(); break;
                                case RedmineKeys.LIMIT: limit = reader.ReadAsInt32().GetValueOrDefault(); break;
                                default:
                                    list = reader.ReadAsCollection<T>(); break;
                            }
                        }
                    }
                    return new PaginatedResult<T>(list, total, offset, limit);
                }
            }
        }

        public string Serialize<T>(T obj) where T : class
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    
                    var ser = obj as IJsonSerializable;

                    if (ser == null)
                    {
                        throw new RedmineException($"object '{typeof(T)}' should implement IJsonSerializable.");
                    }

                    ser.ReadJson(writer);

                    return sb.ToString();
                }
            }

        }
    }
}