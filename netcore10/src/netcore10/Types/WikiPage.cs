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
    /// Availability 2.2
    /// </summary>
    [XmlRoot(RedmineKeys.WIKI_PAGE)]
    public class WikiPage : Identifiable<WikiPage>, IXmlSerializable, IEquatable<WikiPage>, IJsonSerializable
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.TITLE)]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.TEXT)]
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.COMMENTS)]
        public string Comments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.VERSION)]
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.AUTHOR)]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement(RedmineKeys.UPDATED_ON)]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        [XmlArray(RedmineKeys.ATTACHMENTS)]
        [XmlArrayItem(RedmineKeys.ATTACHMENT)]
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Sets the uploads.
        /// </summary>
        /// <value>
        /// The uploads.
        /// </value>
        /// <remarks>Availability starting with redmine version 3.3</remarks>
        [XmlArray(RedmineKeys.UPLOADS)]
        [XmlArrayItem(RedmineKeys.UPLOAD)]
        public IList<Attachment> Uploads { get; set; }

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

                    case RedmineKeys.TITLE: Title = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.TEXT: Text = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.VERSION: Version = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.TEXT, Text);
            writer.WriteElementString(RedmineKeys.COMMENTS, Comments);
            writer.WriteValueOrEmpty<int>(Version, RedmineKeys.VERSION);
            writer.WriteArray(Uploads, RedmineKeys.UPLOADS);
        }

        #endregion

        #region Implementation of IJsonSerialization
        public void ReadJson(JsonWriter writer)
        {
            throw new NotImplementedException();
        }

        public void WriteJson(JsonReader reader)
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

                    case RedmineKeys.TITLE: Title = reader.ReadAsString(); break;

                    case RedmineKeys.TEXT: Text = reader.ReadAsString(); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadAsString(); break;

                    case RedmineKeys.VERSION: Version = reader.ReadAsInt(); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadAsCollection<Attachment>(); break;

                    default: reader.Read(); break;
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<WikiPage>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(WikiPage other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id
                && Title == other.Title
                && Text == other.Text
                && Comments == other.Comments
                && Version == other.Version
                && Author == other.Author
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Text, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Version, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
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
            return $"[WikiPage: {base.ToString()}, Title={Title}, Text={Text}, Comments={Comments}, Version={Version}, Author={Author}, CreatedOn={CreatedOn}, UpdatedOn={UpdatedOn}, Attachments={Attachments}]";
        }
    }
}