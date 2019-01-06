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
using System.Xml.Schema;
using System.Xml.Serialization;
using RedmineApi.Core.Internals;
using Newtonsoft.Json;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Extensions;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// Support for adding attachments through the REST API is added in Redmine 1.4.0.
    /// </summary>
    [XmlRoot(RedmineKeys.UPLOAD)]
    public class Upload : IEquatable<Upload>, IJsonSerializable
    {
        /// <summary>
        /// Gets or sets the uploaded token.
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.TOKEN)]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// Maximum allowed file size (1024000).
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.FILENAME)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.CONTENT_TYPE)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the file description. (Undocumented feature)
        /// </summary>
        /// <value>The file descroütopm.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public string Description { get; set; }

        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() { return null; }
        #endregion

        #region Implementation of IJsonSerialization
        public void WriteJson(JsonWriter writer)
        {
                writer.WriteProperty(RedmineKeys.TOKEN, Token);
                writer.WriteProperty(RedmineKeys.CONTENT_TYPE, ContentType);
                writer.WriteProperty(RedmineKeys.FILENAME, FileName);
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
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
                
                switch(reader.Value)
                {
                    case RedmineKeys.CONTENT_TYPE:   ContentType = reader.ReadAsString();break;
                    case    RedmineKeys.FILENAME:  FileName = reader.ReadAsString();break;
                    case  RedmineKeys.TOKEN:    Token = reader.ReadAsString();break;
                    case    RedmineKeys.DESCRIPTION:   Description = reader.ReadAsString();break;
                    default: reader.Read(); break;
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<Upload>
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Upload other)
        {
            return other != null
                && Token.Equals(other.Token)
                && FileName.Equals(other.FileName)
                && Description.Equals(other.Description)
                && ContentType.Equals(other.ContentType);
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

            return Equals(obj as Upload);
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
                hashCode = HashCodeHelper.GetHashCode(Token, hashCode);
                hashCode = HashCodeHelper.GetHashCode(FileName, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
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
            return $"[Upload: Token={Token}, FileName={FileName}, ContentType={ContentType}, Description={Description}]";
        }
    }
}