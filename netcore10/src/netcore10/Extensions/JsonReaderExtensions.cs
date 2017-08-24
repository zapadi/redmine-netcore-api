using Newtonsoft.Json;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedmineApi.Core.Extensions
{
    public static partial class JsonExtensions
    {
        public static List<T> ReadAsCollection<T>(this JsonReader reader) where T : class, new()
        {
            List<T> col = new List<T>();
            // reader.Read();

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

                if (reader.Read())
                {
                    ser.Deserialize(reader);
                }
                T des = (T)ser;
                if (des != default(T))
                {
                    col.Add(des);
                }
            }

            return col;
        }

        //public static List<T> ReadElementContentAsCollection<T>(this XmlReader reader) where T : class
        //{
        //    var result = new List<T>();
        //    var serializer = new XmlSerializer(typeof(T));
        //    var xml = reader.ReadOuterXml();
        //    using (TextReader sr = new StringReader(xml))
        //    {
        //        using (var xmlTextReader = XmlReader.Create(sr))
        //        {
        //            xmlTextReader.ReadStartElement();
        //            while (!xmlTextReader.EOF)
        //            {
        //                if (xmlTextReader.NodeType == XmlNodeType.EndElement)
        //                {
        //                    xmlTextReader.ReadEndElement();
        //                    continue;
        //                }

        //                T obj;

        //                if (xmlTextReader.IsEmptyElement && xmlTextReader.HasAttributes)
        //                {
        //                    obj = serializer.Deserialize(xmlTextReader) as T;
        //                }
        //                else
        //                {
        //                    var subTree = xmlTextReader.ReadSubtree();
        //                    obj = serializer.Deserialize(subTree) as T;
        //                }
        //                if (obj != null)
        //                {
        //                    result.Add(obj);
        //                }

        //                if (!xmlTextReader.IsEmptyElement)
        //                {
        //                    xmlTextReader.Read();
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

    }
}
