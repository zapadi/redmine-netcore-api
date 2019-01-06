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
    [XmlRoot(RedmineKeys.VALUE)]
    public class CustomFieldValue : IEquatable<CustomFieldValue>, IJsonSerializable
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string Info { get; set; }

        #region Implementation of IJsonSerialization
        public void WriteJson(JsonWriter writer){}

        public void ReadJson(JsonReader reader)
        {
            if(reader.TokenType == JsonToken.PropertyName)
            {
                return;
            }
            
            if (reader.TokenType == JsonToken.String)
            {
                Info = reader.Value as string;
            }
        }
        #endregion

        #region Implementation of IEquatable<CustomField>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CustomFieldValue other)
        {
			if (other == null)
			{
				return false;
			}

            return Info == other.Info;
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

            return Equals(obj as CustomFieldValue);
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
                hashCode = HashCodeHelper.GetHashCode(Info, hashCode);
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
            return $"[CustomFieldValue: Info={Info}]";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CustomFieldValue Clone()
        {
            var customFieldValue = new CustomFieldValue { Info = Info };
            return customFieldValue;
        }
    }
}