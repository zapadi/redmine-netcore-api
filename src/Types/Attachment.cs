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
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.ATTACHMENT)]
    public class Attachment : Identifiable<Attachment>, IXmlSerializable, IEquatable<Attachment>, IJsonSerializable
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.FILENAME)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        [XmlElement(RedmineKeys.FILESIZE)]
        public int FileSize { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        [XmlElement(RedmineKeys.CONTENT_TYPE)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the content URL.
        /// </summary>
        /// <value>The content URL.</value>
        [XmlElement(RedmineKeys.CONTENT_URL)]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlElement(RedmineKeys.AUTHOR)]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON)]
        public DateTime? CreatedOn { get; set; }

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

                    case RedmineKeys.FILENAME: FileName = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.FILESIZE: FileSize = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.CONTENT_URL: ContentUrl = reader.ReadElementContentAsString(); break;

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

        #region Implementation of IJsonSerializable

        public void WriteJson(JsonWriter writer) 
        {
            using(new JsonObject(writer, RedmineKeys.ATTACHMENT))
            {
                writer.WriteProperty(RedmineKeys.FILENAME, FileName);
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
            }
         }

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

                    case RedmineKeys.FILENAME: FileName = reader.ReadAsString(); break;

                    case RedmineKeys.FILESIZE: FileSize = reader.ReadAsInt(); break;

                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadAsString(); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;

                    case RedmineKeys.CONTENT_URL: ContentUrl = reader.ReadAsString(); break;

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
        public bool Equals(Attachment other)
        {
            if (other == null)
            {
                return false;
            }

            return (Id == other.Id
                && FileName == other.FileName
                && FileSize == other.FileSize
                && ContentType == other.ContentType
                && Author == other.Author
                && CreatedOn == other.CreatedOn
                && Description == other.Description
                && ContentUrl == other.ContentUrl);
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
                hashCode = HashCodeHelper.GetHashCode(FileName, hashCode);
                hashCode = HashCodeHelper.GetHashCode(FileSize, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ContentUrl, hashCode);

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
            return $"[Attachment: {base.ToString()}, FileName={FileName}, FileSize={FileSize}, ContentType={ContentType}, Description={Description}, ContentUrl={ContentUrl}, Author={Author}, CreatedOn={CreatedOn}]";
        }
    }
}