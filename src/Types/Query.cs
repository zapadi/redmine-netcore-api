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
using System.Xml;
using System.Xml.Serialization;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Internals;
using Newtonsoft.Json;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.QUERY)]
    public class Query : IdentifiableName, IEquatable<Query>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is public.
        /// </summary>
        /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
        [XmlElement(RedmineKeys.IS_PUBLIC)]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        [XmlElement(RedmineKeys.PROJECT_ID)]
        public int? ProjectId { get; set; }

        #region Implementation of IXmlSerializable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
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
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.IS_PUBLIC: IsPublic = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.PROJECT_ID: ProjectId = reader.ReadElementContentAsNullableInt(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer) { }
        #endregion

        #region Implementation of IJsonSerialization
        public override void WriteJson(JsonWriter writer) { }

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

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;

                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;

                    case RedmineKeys.IS_PUBLIC: IsPublic = reader.ReadAsBool(); break;

                    case RedmineKeys.PROJECT_ID: ProjectId = reader.ReadAsInt(); break;

                    default: reader.Read(); break;
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<Query>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Query other)
        {
            if (other == null)
            {
                return false;
            }

            return (other.Id == Id && other.Name == Name && other.IsPublic == IsPublic && other.ProjectId == ProjectId);
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
                hashCode = HashCodeHelper.GetHashCode(IsPublic, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ProjectId, hashCode);
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
            return $"[Query: {base.ToString()}, IsPublic={IsPublic}, ProjectId={ProjectId}]";
        }
    }
}