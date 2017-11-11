/*
   Copyright 2011 - 2017 Adrian Popescu.

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
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Internals;
using System.Linq;
using Newtonsoft.Json;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.CUSTOM_FIELD)]
    public class IssueCustomField : IdentifiableName, IEquatable<IssueCustomField>
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlArray(RedmineKeys.VALUE)]
        [XmlArrayItem(RedmineKeys.VALUE)]
        public IList<CustomFieldValue> Values { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(RedmineKeys.MULTIPLE)]
        public bool Multiple { get; set; }

        #region Implementation of IXmlSerializable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            Id = Convert.ToInt32(reader.GetAttribute(RedmineKeys.ID));
            Name = reader.GetAttribute(RedmineKeys.NAME);

            Multiple = reader.ReadAttributeAsBoolean(RedmineKeys.MULTIPLE);
            reader.Read();

            if (string.IsNullOrEmpty(reader.GetAttribute("type")))
            {
                Values = new List<CustomFieldValue> { new CustomFieldValue { Info = reader.ReadElementContentAsString() } };
            }
            else
            {
                var result = reader.ReadElementContentAsCollection<CustomFieldValue>();
                Values = result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            if (Values == null)
            {
                return;
            }

            var itemsCount = Values.Count;

            writer.WriteAttributeString(RedmineKeys.ID, Id.ToString(CultureInfo.InvariantCulture));
            if (itemsCount > 1)
            {
                writer.WriteArrayStringElement(Values, RedmineKeys.VALUE, GetValue);
            }
            else
            {
                writer.WriteElementString(RedmineKeys.VALUE, itemsCount > 0 ? Values[0].Info : null);
            }
        }
        #endregion

        #region Implementation of IJsonSerialization
        public override void WriteJson(JsonWriter writer)
        {
           if (Values == null) return ;
            var itemsCount = Values.Count;

            writer.WriteStartObject();
            writer.WriteProperty(RedmineKeys.ID, Id);
            writer.WriteProperty(RedmineKeys.NAME, Name);

            if (itemsCount > 1)
            {
                writer.WritePropertyName(RedmineKeys.VALUE);
                writer.WriteStartArray();
                foreach (var cfv in Values)
                {
                    writer.WriteValue(cfv.Info);
                }
                writer.WriteEndArray();

                writer.WriteProperty(RedmineKeys.MULTIPLE, Multiple.ToString().ToLowerInvariant());
            }
            else
            {
                writer.WriteProperty(RedmineKeys.VALUE, itemsCount > 0 ? Values[0].Info : null);
            }
            writer.WriteEndObject();
        }

        public override void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch(reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    case RedmineKeys.MULTIPLE: Multiple = reader.ReadAsBool(); break;
                    case RedmineKeys.VALUE:

                        Values = reader.ReadAsArray<CustomFieldValue>();
                     break;
                    default :  reader.Read(); break;
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IssueCustomField other)
        {
            if (other == null)
            {
                return false;
            }

            return (Id == other.Id
                && Name == other.Name
                && Multiple == other.Multiple
                && Values == other.Values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IssueCustomField Clone()
        {
            var issueCustomField = new IssueCustomField { Multiple = Multiple, Values = Values.Select(x => x.Clone()).ToList() };
            return issueCustomField;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Values, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Multiple, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[IssueCustomField: {base.ToString()} Values={Values}, Multiple={Multiple}]";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetValue(object item)
        {
            return ((CustomFieldValue)item).Info;
        }
    }
}