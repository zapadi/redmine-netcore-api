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
using System.Xml.Schema;
using System.Xml.Serialization;
using RedmineApi.Core.Internals;
using Newtonsoft.Json;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Extensions;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.ISSUE)]
    public class IssueChild : Identifiable<IssueChild>, IXmlSerializable, IEquatable<IssueChild>, IJsonSerializable
    {
        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        [XmlElement(RedmineKeys.TRACKER)]
        public IdentifiableName Tracker { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [XmlElement(RedmineKeys.SUBJECT)]
        public String Subject { get; set; }

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
            Id = Convert.ToInt32(reader.GetAttribute(RedmineKeys.ID));
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
                    case RedmineKeys.TRACKER: Tracker = new IdentifiableName(reader); break;

                    case RedmineKeys.SUBJECT: Subject = reader.ReadElementContentAsString(); break;

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

                    case RedmineKeys.TRACKER: Tracker = new IdentifiableName(reader); break;

                    case RedmineKeys.SUBJECT: Subject = reader.ReadAsString(); break;

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
        public bool Equals(IssueChild other)
        {
            if (other == null)
            {
                return false;
            }

            return (Id == other.Id
                && Tracker == other.Tracker
                && Subject == other.Subject);
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
                hashCode = HashCodeHelper.GetHashCode(Tracker, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Subject, hashCode);
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
            return $"[IssueChild: {base.ToString()}, Tracker={Tracker}, Subject={Subject}]";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IssueChild Clone()
        {
            var issueChild = new IssueChild { Subject = Subject, Tracker = Tracker };
            return issueChild;
        }
    }
}
