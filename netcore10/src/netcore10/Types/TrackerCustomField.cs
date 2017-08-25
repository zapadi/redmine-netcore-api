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

using System.Xml;
using System.Xml.Serialization;
using RedmineApi.Core.Extensions;
using Newtonsoft.Json;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.TRACKER)]
    public class TrackerCustomField : Tracker
    {
        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            Name = reader.GetAttribute(RedmineKeys.NAME);
            reader.Read();
        }
        #endregion

        #region Implementation of IJsonSerialization
        
        public override void WriteJson(JsonReader reader)
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

                    default: reader.Read(); break;
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public override string ToString()
        {
            return $"[TrackerCustomField: {base.ToString()}]";
        }
    }
}