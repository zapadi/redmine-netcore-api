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
using RedmineApi.Core.Types;

namespace RedmineApi.Core.Serializers
{
    internal static class RedmineSerializer
    {
        private static readonly Dictionary<MimeType, IRedmineSerializer> serializers = new Dictionary<MimeType, IRedmineSerializer>
        {
            [MimeType.Xml] = new RedmineXmlSerializer(),
            [MimeType.Json] = new RedmineJsonSerializer()
        };

        /// <summary>
        ///     Serializes the specified type T and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="RedmineApi.Core.Exceptions.RedmineException">Serialization error</exception>
        public static string Serialize<T>(T obj, MimeType mimeFormat) where T : class, new()
        {
            return serializers[mimeFormat].Serialize(obj);
        }

        /// <summary>
        ///     Deserializes the XML/JSON document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="RedmineApi.Core.Exceptions.RedmineException">
        ///     Could not deserialize null!
        ///     or
        ///     Deserialization error
        /// </exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T Deserialize<T>(string response, MimeType mimeFormat) where T :  new()
        {
            return serializers[mimeFormat].Deserialize<T>(response);
        }

        /// <summary>
        ///     Deserializes the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="RedmineApi.Core.Exceptions.RedmineException">
        ///     Could not deserialize null!
        ///     or
        ///     Deserialization error
        /// </exception>
        public static PaginatedResult<T> DeserializeList<T>(string response, MimeType mimeFormat)
            where T : class, new()
        {
            return serializers[mimeFormat].DeserializeList<T>(response);
        }

        public static int Count<T>(string response, MimeType mimeFormat)
            where T :  new()
        {
            return serializers[mimeFormat].Count<T>(response);
        }
    }
}