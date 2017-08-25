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
    /// Availability 1.1
    /// </summary>
    [XmlRoot(RedmineKeys.TIME_ENTRY)]
    public class TimeEntry : Identifiable<TimeEntry>, IEquatable<TimeEntry>, IXmlSerializable, IJsonSerializable
    {
        private string comments;

        /// <summary>
        /// Gets or sets the issue id to log time on.
        /// </summary>
        /// <value>The issue id.</value>
        [XmlAttribute(RedmineKeys.ISSUE)]
        public IdentifiableName Issue { get; set; }

        /// <summary>
        /// Gets or sets the project id to log time on.
        /// </summary>
        /// <value>The project id.</value>
        [XmlAttribute(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the date the time was spent (default to the current date).
        /// </summary>
        /// <value>The spent on.</value>
        [XmlAttribute(RedmineKeys.SPENT_ON)]
        public DateTime? SpentOn { get; set; }

        /// <summary>
        /// Gets or sets the number of spent hours.
        /// </summary>
        /// <value>The hours.</value>
        [XmlAttribute(RedmineKeys.HOURS)]
        public decimal Hours { get; set; }

        /// <summary>
        /// Gets or sets the activity id of the time activity. This parameter is required unless a default activity is defined in Redmine..
        /// </summary>
        /// <value>The activity id.</value>
        [XmlAttribute(RedmineKeys.ACTIVITY)]
        public IdentifiableName Activity { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [XmlAttribute(RedmineKeys.USER)]
        public IdentifiableName User { get; set; }

        /// <summary>
        /// Gets or sets the short description for the entry (255 characters max).
        /// </summary>
        /// <value>The comments.</value>
        [XmlAttribute(RedmineKeys.COMMENTS)]
        public String Comments
        {
            get { return comments; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length > 255)
                    {
                        value = value.Substring(0, 255);
                    }
                }
                comments = value;
            }
        }

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
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TimeEntry Clone()
        {
            var timeEntry = new TimeEntry { Activity = Activity, Comments = Comments, Hours = Hours, Issue = Issue, Project = Project, SpentOn = SpentOn, User = User, CustomFields = CustomFields };
            return timeEntry;
        }

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

                    case RedmineKeys.ISSUE_ID: Issue = new IdentifiableName(reader); break;

                    case RedmineKeys.ISSUE: Issue = new IdentifiableName(reader); break;

                    case RedmineKeys.PROJECT_ID: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.SPENT_ON: SpentOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

                    case RedmineKeys.HOURS: Hours = reader.ReadElementContentAsDecimal(); break;

                    case RedmineKeys.ACTIVITY_ID: Activity = new IdentifiableName(reader); break;

                    case RedmineKeys.ACTIVITY: Activity = new IdentifiableName(reader); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;

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
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteIdIfNotNull(Issue, RedmineKeys.ISSUE_ID);
            writer.WriteIdIfNotNull(Project, RedmineKeys.PROJECT_ID);

            if (!SpentOn.HasValue)
            {
                SpentOn = DateTime.Now;
            }

            writer.WriteDateOrEmpty(SpentOn, RedmineKeys.SPENT_ON);
            writer.WriteValueOrEmpty<decimal>(Hours, RedmineKeys.HOURS);
            writer.WriteIdIfNotNull(Activity, RedmineKeys.ACTIVITY_ID);
            writer.WriteElementString(RedmineKeys.COMMENTS, Comments);

            writer.WriteArray(CustomFields, RedmineKeys.CUSTOM_FIELDS);
        }
        #endregion

        #region Implementation of IJsonSerialization
        public void WriteJson(JsonWriter writer)
        {
            //TODO: implement
            throw new NotImplementedException();
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

                    case RedmineKeys.ISSUE_ID: Issue = new IdentifiableName(reader); break;

                    case RedmineKeys.ISSUE: Issue = new IdentifiableName(reader); break;

                    case RedmineKeys.PROJECT_ID: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.SPENT_ON: SpentOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

                    case RedmineKeys.HOURS: Hours = reader.ReadAsDecimal().GetValueOrDefault(); break;

                    case RedmineKeys.ACTIVITY_ID: Activity = new IdentifiableName(reader); break;

                    case RedmineKeys.ACTIVITY: Activity = new IdentifiableName(reader); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadAsString(); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;

                    default: reader.Read(); break;
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<TimeEntry>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TimeEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return (Id == other.Id
                && Issue == other.Issue
                && Project == other.Project
                && SpentOn == other.SpentOn
                && Hours == other.Hours
                && Activity == other.Activity
                && Comments == other.Comments
                && User == other.User
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && CustomFields == other.CustomFields);
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
                hashCode = HashCodeHelper.GetHashCode(Issue, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(SpentOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Hours, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Activity, hashCode);
                hashCode = HashCodeHelper.GetHashCode(User, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
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
            return $"[TimeEntry: {base.ToString()}, Issue={Issue}, Project={Project}, SpentOn={SpentOn}, Hours={Hours}, Activity={Activity}, User={User}, Comments={Comments}, CreatedOn={CreatedOn}, UpdatedOn={UpdatedOn}, CustomFields={CustomFields}]";
        }
    }
}