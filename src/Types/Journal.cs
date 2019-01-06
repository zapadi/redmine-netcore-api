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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Internals;
using Newtonsoft.Json;
using RedmineApi.Core.Serializers;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.JOURNAL)]
    public class Journal : Identifiable<Journal>, IEquatable<Journal>, IXmlSerializable, IJsonSerializable
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [XmlElement(RedmineKeys.USER)]
        public IdentifiableName User { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        [XmlElement(RedmineKeys.NOTES)]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>
        /// The created on.
        /// </value>
        [XmlElement(RedmineKeys.CREATED_ON, IsNullable = true)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        [XmlArray(RedmineKeys.DETAILS)]
        [XmlArrayItem(RedmineKeys.DETAIL)]
        public IList<Detail> Details { get; set; }

        #region Implementation of IXmlSerializable
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() { return null; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            reader.Read();

            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

                    case RedmineKeys.NOTES: Notes = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.DETAILS: Details = reader.ReadElementContentAsCollection<Detail>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer) { }
        #endregion

        #region Implementation of IJsonSerialization
        public void WriteJson(JsonWriter writer) { }

        public void ReadJson(JsonReader reader)
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

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;

                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

                    case RedmineKeys.NOTES: Notes = reader.ReadAsString(); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.DETAILS: Details = reader.ReadAsCollection<Detail>(); break;

                    default: reader.Read(); break;
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
        public bool Equals(Journal other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id
                && User == other.User
                && Notes == other.Notes
                && CreatedOn == other.CreatedOn
                && Details == other.Details;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals(obj as Journal);
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
                hashCode = HashCodeHelper.GetHashCode(User, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Notes, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Details, hashCode);
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
            return $"[Journal: Id={Id}, User={User}, Notes={Notes}, CreatedOn={CreatedOn}, Details={Details}]";
        }
    }
}