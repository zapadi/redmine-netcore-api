﻿/*
   Copyright 2011 - 2017 Adrian Popescu, Dorin Huzum.

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
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Serializers;

namespace RedmineApi.Core.Types
{
    /// <summary>
    /// Available as of 1.1 :
    ///include: fetch associated data (optional).
    ///Possible values: children, attachments, relations, changesets and journals. To fetch multiple associations use comma (e.g ?include=relations,journals).
    /// See Issue journals for more information.
    /// </summary>
    [XmlRoot(RedmineKeys.ISSUE)]
    public class Issue : Identifiable<Issue>, IXmlSerializable, IEquatable<Issue>, IJsonSerializable
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [XmlElement(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        [XmlElement(RedmineKeys.TRACKER)]
        public IdentifiableName Tracker { get; set; }

        /// <summary>
        /// Gets or sets the status.Possible values: open, closed, * to get open and closed issues, status id
        /// </summary>
        /// <value>The status.</value>
        [XmlElement(RedmineKeys.STATUS)]
        public IdentifiableName Status { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        [XmlElement(RedmineKeys.PRIORITY)]
        public IdentifiableName Priority { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlElement(RedmineKeys.AUTHOR)]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        [XmlElement(RedmineKeys.CATEGORY)]
        public IdentifiableName Category { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [XmlElement(RedmineKeys.SUBJECT)]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        [XmlElement(RedmineKeys.START_DATE, IsNullable = true)]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>The due date.</value>
        [XmlElement(RedmineKeys.DUE_DATE, IsNullable = true)]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the done ratio.
        /// </summary>
        /// <value>The done ratio.</value>
        [XmlElement(RedmineKeys.DONE_RATIO, IsNullable = true)]
        public float? DoneRatio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [private notes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [private notes]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement(RedmineKeys.PRIVATE_NOTES)]
        public bool PrivateNotes { get; set; }

        /// <summary>
        /// Gets or sets the estimated hours.
        /// </summary>
        /// <value>The estimated hours.</value>
        [XmlElement(RedmineKeys.ESTIMATED_HOURS, IsNullable = true)]
        public float? EstimatedHours { get; set; }

        /// <summary>
        /// Gets or sets the hours spent on the issue.
        /// </summary>
        /// <value>The hours spent on the issue.</value>
        [XmlElement(RedmineKeys.SPENT_HOURS, IsNullable = true)]
        public float? SpentHours { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

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
        /// Gets or sets the closed on.
        /// </summary>
        /// <value>The closed on.</value>
        [XmlElement(RedmineKeys.CLOSED_ON, IsNullable = true)]
        public DateTime? ClosedOn { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        [XmlElement(RedmineKeys.NOTES)]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user to assign the issue to (currently no mechanism to assign by name).
        /// </summary>
        /// <value>
        /// The assigned to.
        /// </value>
        [XmlElement(RedmineKeys.ASSIGNED_TO)]
        public IdentifiableName AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the parent issue id. Only when a new issue is created this property shall be used.
        /// </summary>
        /// <value>
        /// The parent issue id.
        /// </value>
        [XmlElement(RedmineKeys.PARENT)]
        public IdentifiableName ParentIssue { get; set; }

        /// <summary>
        /// Gets or sets the fixed version.
        /// </summary>
        /// <value>
        /// The fixed version.
        /// </value>
        [XmlElement(RedmineKeys.FIXED_VERSION)]
        public IdentifiableName FixedVersion { get; set; }

        /// <summary>
        /// indicate whether the issue is private or not
        /// </summary>
        /// <value>
        /// <c>true</c> if this issue is private; otherwise, <c>false</c>.
        /// </value>
        [XmlElement(RedmineKeys.IS_PRIVATE)]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Returns the sum of spent hours of the task and all the subtasks.
        /// </summary>
        /// <remarks>Availability starting with redmine version 3.3</remarks>
        [XmlElement(RedmineKeys.TOTAL_SPENT_HOURS)]
        public float? TotalSpentHours { get; set; }

        /// <summary>
        /// Returns the sum of estimated hours of task and all the subtasks.
        /// </summary>
        /// <remarks>Availability starting with redmine version 3.3</remarks>
        [XmlElement(RedmineKeys.TOTAL_ESTIMATED_HOURS)]
        public float? TotalEstimatedHours { get; set; }

        /// <summary>
        /// Gets or sets the journals.
        /// </summary>
        /// <value>
        /// The journals.
        /// </value>
        [XmlArray(RedmineKeys.JOURNALS)]
        [XmlArrayItem(RedmineKeys.JOURNAL)]
        public IList<Journal> Journals { get; set; }

        /// <summary>
        /// Gets or sets the changesets.
        /// </summary>
        /// <value>
        /// The changesets.
        /// </value>
        [XmlArray(RedmineKeys.CHANGESETS)]
        [XmlArrayItem(RedmineKeys.CHANGESET)]
        public IList<ChangeSet>? Changesets { get; set; }

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
        /// Gets or sets the issue relations.
        /// </summary>
        /// <value>
        /// The issue relations.
        /// </value>
        [XmlArray(RedmineKeys.RELATIONS)]
        [XmlArrayItem(RedmineKeys.RELATION)]
        public IList<IssueRelation> Relations { get; set; }

        /// <summary>
        /// Gets or sets the issue children.
        /// </summary>
        /// <value>
        /// The issue children.
        /// NOTE: Only Id, tracker and subject are filled.
        /// </value>
        [XmlArray(RedmineKeys.CHILDREN)]
        [XmlArrayItem(RedmineKeys.ISSUE)]
        public IList<IssueChild> Children { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachment.
        /// </value>
        [XmlArray(RedmineKeys.UPLOADS)]
        [XmlArrayItem(RedmineKeys.UPLOAD)]
        public IList<Upload> Uploads { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlArray(RedmineKeys.WATCHERS)]
        [XmlArrayItem(RedmineKeys.WATCHER)]
        public IList<Watcher> Watchers { get; set; }

        #region Implementation of IXmlSerializable
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

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
                    case RedmineKeys.ID:
                        Id = reader.ReadElementContentAsInt();
                        break;

                    case RedmineKeys.PROJECT:
                        Project = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.TRACKER:
                        Tracker = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.STATUS:
                        Status = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.PRIORITY:
                        Priority = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.AUTHOR:
                        Author = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.ASSIGNED_TO:
                        AssignedTo = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.CATEGORY:
                        Category = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.PARENT:
                        ParentIssue = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.FIXED_VERSION:
                        FixedVersion = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.PRIVATE_NOTES:
                        PrivateNotes = reader.ReadElementContentAsBoolean();
                        break;

                    case RedmineKeys.IS_PRIVATE:
                        IsPrivate = reader.ReadElementContentAsBoolean();
                        break;

                    case RedmineKeys.SUBJECT:
                        Subject = reader.ReadElementContentAsString();
                        break;

                    case RedmineKeys.NOTES:
                        Notes = reader.ReadElementContentAsString();
                        break;

                    case RedmineKeys.DESCRIPTION:
                        Description = reader.ReadElementContentAsString();
                        break;

                    case RedmineKeys.START_DATE:
                        StartDate = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case RedmineKeys.DUE_DATE:
                        DueDate = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case RedmineKeys.DONE_RATIO:
                        DoneRatio = reader.ReadElementContentAsNullableFloat();
                        break;

                    case RedmineKeys.ESTIMATED_HOURS:
                        EstimatedHours = reader.ReadElementContentAsNullableFloat();
                        break;

                    case RedmineKeys.TOTAL_ESTIMATED_HOURS:
                        TotalEstimatedHours = reader.ReadElementContentAsNullableFloat();
                        break;

                    case RedmineKeys.TOTAL_SPENT_HOURS:
                        TotalSpentHours = reader.ReadElementContentAsNullableFloat();
                        break;

                    case RedmineKeys.SPENT_HOURS:
                        SpentHours = reader.ReadElementContentAsNullableFloat();
                        break;

                    case RedmineKeys.CREATED_ON:
                        CreatedOn = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case RedmineKeys.UPDATED_ON:
                        UpdatedOn = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case RedmineKeys.CLOSED_ON:
                        ClosedOn = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case RedmineKeys.CUSTOM_FIELDS:
                        CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>();
                        break;

                    case RedmineKeys.ATTACHMENTS:
                        Attachments = reader.ReadElementContentAsCollection<Attachment>();
                        break;

                    case RedmineKeys.RELATIONS:
                        Relations = reader.ReadElementContentAsCollection<IssueRelation>();
                        break;

                    case RedmineKeys.JOURNALS:
                        Journals = reader.ReadElementContentAsCollection<Journal>();
                        break;

                    case RedmineKeys.CHANGESETS:
                        Changesets = reader.ReadElementContentAsCollection<ChangeSet>();
                        break;

                    case RedmineKeys.CHILDREN:
                        Children = reader.ReadElementContentAsCollection<IssueChild>();
                        break;

                    case RedmineKeys.WATCHERS:
                        Watchers = reader.ReadElementContentAsCollection<Watcher>();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.SUBJECT, Subject);
            writer.WriteElementString(RedmineKeys.NOTES, Notes);

            if (Id != 0)
            {
                writer.WriteElementString(RedmineKeys.PRIVATE_NOTES, PrivateNotes.ToString().ToLowerInvariant());
            }

            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
            writer.WriteStartElement(RedmineKeys.IS_PRIVATE);
            writer.WriteValue(IsPrivate.ToString().ToLowerInvariant());
            writer.WriteEndElement();

            writer.WriteIdIfNotNull(Project, RedmineKeys.PROJECT_ID);
            writer.WriteIdIfNotNull(Priority, RedmineKeys.PRIORITY_ID);
            writer.WriteIdIfNotNull(Status, RedmineKeys.STATUS_ID);
            writer.WriteIdIfNotNull(Category, RedmineKeys.CATEGORY_ID);
            writer.WriteIdIfNotNull(Tracker, RedmineKeys.TRACKER_ID);
            writer.WriteIdIfNotNull(AssignedTo, RedmineKeys.ASSIGNED_TO_ID);
            writer.WriteIdIfNotNull(ParentIssue, RedmineKeys.PARENT_ISSUE_ID);
            writer.WriteIdIfNotNull(FixedVersion, RedmineKeys.FIXED_VERSION_ID);

            writer.WriteValueOrEmpty(EstimatedHours, RedmineKeys.ESTIMATED_HOURS);
            writer.WriteIfNotDefaultOrNull(DoneRatio, RedmineKeys.DONE_RATIO);
            writer.WriteDateOrEmpty(StartDate, RedmineKeys.START_DATE);
            writer.WriteDateOrEmpty(DueDate, RedmineKeys.DUE_DATE);
            writer.WriteDateOrEmpty(UpdatedOn, RedmineKeys.UPDATED_ON);

            writer.WriteArray(Uploads, RedmineKeys.UPLOADS);
            writer.WriteArray(CustomFields, RedmineKeys.CUSTOM_FIELDS);

            writer.WriteListElements(Watchers as IList<IValue>, RedmineKeys.WATCHER_USER_IDS);
        }
        #endregion

        #region Implementation of IEquatable<Issue>
        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Issue other)
        {
            if (other == null)
            {
                return false;
            }

            return (
                Id == other.Id
                && Project == other.Project
                && Tracker == other.Tracker
                && Status == other.Status
                && Priority == other.Priority
                && Author == other.Author
                && Category == other.Category
                && Subject == other.Subject
                && Description == other.Description
                && StartDate == other.StartDate
                && DueDate == other.DueDate
                && DoneRatio == other.DoneRatio
                && EstimatedHours == other.EstimatedHours
                && CustomFields == other.CustomFields
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && AssignedTo == other.AssignedTo
                && FixedVersion == other.FixedVersion
                && Notes == other.Notes
                && Watchers == other.Watchers
                && ClosedOn == other.ClosedOn
                && SpentHours == other.SpentHours
                && PrivateNotes == other.PrivateNotes
                && Attachments == other.Attachments
                && Changesets == other.Changesets
                && Children == other.Children
                && Journals == other.Journals
                && Relations == other.Relations
            );
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();

            hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Tracker, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Priority, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Category, hashCode);

            hashCode = HashCodeHelper.GetHashCode(Subject, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
            hashCode = HashCodeHelper.GetHashCode(StartDate, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
            hashCode = HashCodeHelper.GetHashCode(DueDate, hashCode);
            hashCode = HashCodeHelper.GetHashCode(DoneRatio, hashCode);

            hashCode = HashCodeHelper.GetHashCode(PrivateNotes, hashCode);
            hashCode = HashCodeHelper.GetHashCode(EstimatedHours, hashCode);
            hashCode = HashCodeHelper.GetHashCode(SpentHours, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);

            hashCode = HashCodeHelper.GetHashCode(Notes, hashCode);
            hashCode = HashCodeHelper.GetHashCode(AssignedTo, hashCode);
            hashCode = HashCodeHelper.GetHashCode(ParentIssue, hashCode);
            hashCode = HashCodeHelper.GetHashCode(FixedVersion, hashCode);
            hashCode = HashCodeHelper.GetHashCode(IsPrivate, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Journals, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);

            hashCode = HashCodeHelper.GetHashCode(Changesets, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Relations, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Children, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Uploads, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Watchers, hashCode);

            return hashCode;
        }
        #endregion

        #region Implementation of IJsonSerializable

        public void WriteJson(JsonWriter writer)
        {
            using(new JsonObject(writer, RedmineKeys.ISSUE))
            {
                writer.WriteProperty(RedmineKeys.SUBJECT, Subject);
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
                writer.WriteProperty(RedmineKeys.NOTES, Notes);
                if (Id != 0)
                {
                    writer.WriteProperty(RedmineKeys.PRIVATE_NOTES, PrivateNotes.ToString().ToLowerInvariant());
                }
                writer.WriteProperty(RedmineKeys.IS_PRIVATE, IsPrivate.ToString().ToLowerInvariant());
                writer.WriteIdIfNotNull( RedmineKeys.PROJECT_ID,Project);
                writer.WriteIdIfNotNull( RedmineKeys.PRIORITY_ID,Priority);
                writer.WriteIdIfNotNull( RedmineKeys.STATUS_ID,Status);
                writer.WriteIdIfNotNull( RedmineKeys.CATEGORY_ID,Category);
                writer.WriteIdIfNotNull( RedmineKeys.TRACKER_ID,Tracker);
                writer.WriteIdIfNotNull( RedmineKeys.ASSIGNED_TO_ID,AssignedTo);
                writer.WriteIdIfNotNull( RedmineKeys.FIXED_VERSION_ID,FixedVersion);
                writer.WriteValueOrEmpty(RedmineKeys.ESTIMATED_HOURS,EstimatedHours);

                writer.WriteIdOrEmpty(RedmineKeys.PARENT_ISSUE_ID, ParentIssue);
                writer.WriteDateOrEmpty( RedmineKeys.START_DATE,StartDate);
                writer.WriteDateOrEmpty( RedmineKeys.DUE_DATE,DueDate);
                writer.WriteDateOrEmpty( RedmineKeys.UPDATED_ON,UpdatedOn);

                if (DoneRatio != null)
                    writer.WriteProperty(RedmineKeys.DONE_RATIO, DoneRatio.Value.ToString(CultureInfo.InvariantCulture));

                if (SpentHours != null)
                    writer.WriteProperty(RedmineKeys.SPENT_HOURS, SpentHours.Value.ToString(CultureInfo.InvariantCulture));

                writer.WriteArray(RedmineKeys.UPLOADS, Uploads);
                writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, CustomFields);

                writer.WriteArrayIds(RedmineKeys.WATCHER_USER_IDS, Watchers);
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
                    case RedmineKeys.ID:
                        Id = reader.ReadAsInt32().GetValueOrDefault();
                        break;

                    case RedmineKeys.PROJECT:
                        Project = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.TRACKER:
                        Tracker = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.STATUS:
                        Status = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.PRIORITY:
                        Priority = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.AUTHOR:
                        Author = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.ASSIGNED_TO:
                        AssignedTo = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.CATEGORY:
                        Category = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.PARENT:
                        ParentIssue = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.FIXED_VERSION:
                        FixedVersion = new IdentifiableName(reader);
                        break;

                    case RedmineKeys.PRIVATE_NOTES:
                        PrivateNotes = reader.ReadAsBoolean().GetValueOrDefault();
                        break;

                    case RedmineKeys.IS_PRIVATE:
                        IsPrivate = reader.ReadAsBoolean().GetValueOrDefault();
                        break;

                    case RedmineKeys.SUBJECT:
                        Subject = reader.ReadAsString();
                        break;

                    case RedmineKeys.NOTES:
                        Notes = reader.ReadAsString();
                        break;

                    case RedmineKeys.DESCRIPTION:
                        Description = reader.ReadAsString();
                        break;

                    case RedmineKeys.START_DATE:
                        StartDate = reader.ReadAsDateTime();
                        break;

                    case RedmineKeys.DUE_DATE:
                        DueDate = reader.ReadAsDateTime();
                        break;

                    case RedmineKeys.DONE_RATIO:
                        DoneRatio = (float?)reader.ReadAsDouble();
                        break;

                    case RedmineKeys.ESTIMATED_HOURS:
                        EstimatedHours = (float?)reader.ReadAsDouble();
                        break;

                    case RedmineKeys.TOTAL_ESTIMATED_HOURS:
                        TotalEstimatedHours = (float?)reader.ReadAsDouble();
                        break;

                    case RedmineKeys.TOTAL_SPENT_HOURS:
                        TotalSpentHours = (float?)reader.ReadAsDouble();
                        break;

                    case RedmineKeys.SPENT_HOURS:
                        SpentHours = (float?)reader.ReadAsDouble();
                        break;

                    case RedmineKeys.CREATED_ON:
                        CreatedOn = reader.ReadAsDateTime();
                        break;

                    case RedmineKeys.UPDATED_ON:
                        UpdatedOn = reader.ReadAsDateTime();
                        break;

                    case RedmineKeys.CLOSED_ON:
                        ClosedOn = reader.ReadAsDateTime();
                        break;

                    case RedmineKeys.CUSTOM_FIELDS:
                        CustomFields = reader.ReadAsCollection<IssueCustomField>();
                        break;

                    case RedmineKeys.ATTACHMENTS:
                        Attachments = reader.ReadAsCollection<Attachment>();
                        break;

                    case RedmineKeys.RELATIONS:
                        Relations = reader.ReadAsCollection<IssueRelation>();
                        break;

                    case RedmineKeys.JOURNALS:
                        Journals = reader.ReadAsCollection<Journal>();
                        break;

                    case RedmineKeys.CHANGESETS:
                        Changesets = reader.ReadAsCollection<ChangeSet>();
                        break;

                    case RedmineKeys.CHILDREN:
                        Children = reader.ReadAsCollection<IssueChild>();
                        break;

                    case RedmineKeys.WATCHERS:
                        Watchers = reader.ReadAsCollection<Watcher>();
                        break;

                    default:
                        reader.Read();
                        break;
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
            return $"[Issue: {base.ToString()}, Project={Project}, Tracker={Tracker}, Status={Status}, Priority={Priority}, Author={Author}, Category={Category}, Subject={Subject}, Description={Description}, StartDate={StartDate}, DueDate={DueDate}, DoneRatio={DoneRatio}, PrivateNotes={PrivateNotes}, EstimatedHours={EstimatedHours}, SpentHours={SpentHours}, CustomFields={CustomFields}, CreatedOn={CreatedOn}, UpdatedOn={UpdatedOn}, ClosedOn={ClosedOn}, Notes={Notes}, AssignedTo={AssignedTo}, ParentIssue={ParentIssue}, FixedVersion={FixedVersion}, IsPrivate={IsPrivate}, Journals={Journals}, Changesets={Changesets}, Attachments={Attachments}, Relations={Relations}, Children={Children}, Uploads={Uploads}, Watchers={Watchers}]";
        }
    }
}
