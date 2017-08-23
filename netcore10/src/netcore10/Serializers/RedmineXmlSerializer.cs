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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Types;


namespace RedmineApi.Core.Internals
{
    internal sealed class RedmineXmlSerializer : IRedmineSerializer
    {
        public T Deserialize<T>(string response) where T : class
        {
            if (string.IsNullOrEmpty(response))
            {
                throw new RedmineException("Could not deserialize null!");
            }

            try
            {
               return FromXML<T>(response);
            }
            catch (Exception ex)
            {
                throw new RedmineException("Deserialization error", ex);
            }
        }

        public PaginatedResult<T> DeserializeList<T>(string response) where T : class, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                {
                    throw new RedmineException("Could not deserialize empty response!");
                }

                return XmlDeserializeList<T>(response);
            }
            catch (Exception ex)
            {
                throw new RedmineException("Deserialization error", ex);
            }
        }

        public string Serialize<T>(T obj) where T: class
        {
            if(obj == default(T))
            {
                throw new RedmineException("Could not serialize null object.");
            }

            try
            {
                return ToXML(obj);
            }
            catch (Exception ex)
            {
                throw new RedmineException("Serialization error", ex);
            }
        }

        /// <summary>
        /// XMLs the deserialize list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private static PaginatedResult<T> XmlDeserializeList<T>(string response) where T : class, new()
        {
            using (TextReader stringReader = new StringReader(response))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    xmlReader.Read();
                    xmlReader.Read();

                    var totalItems = xmlReader.ReadAttributeAsInt(RedmineKeys.TOTAL_COUNT);
                    var offset = xmlReader.ReadAttributeAsInt(RedmineKeys.OFFSET);
                    var result = xmlReader.ReadElementContentAsCollection<T>();
                    return new PaginatedResult<T>(result, totalItems, offset);
                }
            }
        }

        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T">The type of objects to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>
        /// The System.String that contains the XML document.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        // ReSharper disable once InconsistentNaming
        private static string ToXML<T>(T obj) where T : class
        {
            var xws = new XmlWriterSettings { OmitXmlDeclaration = true };
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, xws))
                {
                    var sr = new XmlSerializer(typeof(T));
                    sr.Serialize(xmlWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize.</typeparam>
        /// <param name="xml">The System.String that contains the XML document to deserialize.</param>
        /// <returns>
        /// The T object being deserialized.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        // ReSharper disable once InconsistentNaming
        private static T FromXML<T>(string xml) where T : class
        {
            using (var text = new StringReader(xml))
            {
                var sr = new XmlSerializer(typeof(T));
                return sr.Deserialize(text) as T;
            }
        }
    }
}