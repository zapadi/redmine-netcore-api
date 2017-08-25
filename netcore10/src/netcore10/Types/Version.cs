/*
   Copyright 2016 - 2017 Adrian Popescu.

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
using System.Xml.Serialization;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Internals;
using Newtonsoft.Json;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.VERSION)]
    public class Version : IdentifiableName, IEquatable<Version>
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [XmlElement(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [XmlElement(RedmineKeys.STATUS)]
        public VersionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>The due date.</value>
        [XmlElement(RedmineKeys.DUE_DATE, IsNullable = true)]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the sharing.
        /// </summary>
        /// <value>The sharing.</value>
        [XmlElement(RedmineKeys.SHARING)]
        public VersionSharing Sharing { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON, IsNullable = true)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement(RedmineKeys.UPDATED_ON, IsNullable = true)]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

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

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.STATUS: Status = (VersionStatus)Enum.Parse(typeof(VersionStatus), reader.ReadElementContentAsString(), true); break;

                    case RedmineKeys.DUE_DATE: DueDate = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.SHARING: Sharing = (VersionSharing)Enum.Parse(typeof(VersionSharing), reader.ReadElementContentAsString(), true); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteElementString(RedmineKeys.STATUS, Status.ToString().ToLowerInvariant());
            writer.WriteElementString(RedmineKeys.SHARING, Sharing.ToString().ToLowerInvariant());

            writer.WriteDateOrEmpty(DueDate, RedmineKeys.DUE_DATE);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
        }
        #endregion

        #region Implementation of IJsonSerialization
        public override void WriteJson(JsonWriter writer)
        {
            using(new JsonObject(writer, RedmineKeys.VERSION))
            {
                writer.WriteProperty(RedmineKeys.NAME, Name);
                writer.WriteProperty(RedmineKeys.STATUS, Status.ToString().ToLowerInvariant());
                writer.WriteProperty(RedmineKeys.SHARING, Sharing.ToString().ToLowerInvariant());
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
                writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, DueDate);
            }
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

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;

                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;

                    case RedmineKeys.STATUS: Status = (VersionStatus)Enum.Parse(typeof(VersionStatus), reader.ReadAsString(), true); break;

                    case RedmineKeys.DUE_DATE: DueDate = reader.ReadAsDateTime(); break;

                    case RedmineKeys.SHARING: Sharing = (VersionSharing)Enum.Parse(typeof(VersionSharing), reader.ReadAsString(), true); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;

                    default: reader.Read(); break;
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<Version>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Version other)
        {
            if (other == null)
            {
                return false;
            }

            return (Id == other.Id && Name == other.Name
                && Project == other.Project
                && Description == other.Description
                && Status == other.Status
                && DueDate == other.DueDate
                && Sharing == other.Sharing
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && (CustomFields?.Equals(other.CustomFields) ?? other.CustomFields == null));
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
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
                hashCode = HashCodeHelper.GetHashCode(DueDate, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Sharing, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
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
            return
                $"[Version: {base.ToString()}, Project={Project}, Description={Description}, Status={Status}, DueDate={DueDate}, Sharing={Sharing}, CreatedOn={CreatedOn}, UpdatedOn={UpdatedOn}, CustomFields={CustomFields}]";
        }
    }
}