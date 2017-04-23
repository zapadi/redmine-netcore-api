/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using Redmine.Net.Api.Types;

using System.Collections.Specialized;
using Redmine.Net.Api.Exceptions;

using System.Linq;
using System.Net;
using Redmine.Net.Api.Extensions;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    public class UrlBuilder : IUrlBuild
    {
        private string Host { get; set; }
        private MimeType MimeType { get; set; }
        private string ApiKey { get; set; }
        private string url;
        private string query;
        public static readonly Dictionary<MimeType, string> mimeRepresentation = new Dictionary<MimeType, string>
        {
            [MimeType.Json] = MimeType.Json.ToString().ToLowerInvariant(),
            [MimeType.Xml] = MimeType.Xml.ToString().ToLowerInvariant()
        };
        //  public static IImmutableDictionary<Type, string> TypePath => typePath.ToImmutableDictionary();
        internal static Dictionary<Type, string> typePath = new Dictionary<Type, string>
        {
            [typeof(Issue)] = "issues",
            [typeof(Project)] = "projects",
            [typeof(User)] = "users",
            [typeof(News)] = "news",
            [typeof(Query)] = "queries",
            [typeof(Version)] = "versions",
            [typeof(Attachment)] = "attachments",
            [typeof(IssueRelation)] = "relations",
            [typeof(TimeEntry)] = "time_entries",
            [typeof(IssueStatus)] = "issue_statuses",
            [typeof(Tracker)] = "trackers",
            [typeof(IssueCategory)] = "issue_categories",
            [typeof(Role)] = "roles",
            [typeof(ProjectMembership)] = "memberships",
            [typeof(Group)] = "groups",
            [typeof(TimeEntryActivity)] = "enumerations/time_entry_activities",
            [typeof(IssuePriority)] = "enumerations/issue_priorities",
            [typeof(Watcher)] = "watchers",
            [typeof(IssueCustomField)] = "custom_fields",
            [typeof(CustomField)] = "custom_fields"
        };

        private UrlBuilder()
        {
        }

        public static UrlBuilder Create(string host, string apiKey, MimeType mimeType)
        {
            var b = new UrlBuilder { Host = host, MimeType = mimeType, ApiKey = apiKey };
            return b;
        }

        private const string URL_FORMAT = "{0}/{1}.{2}";
        private const string REQUEST_URL_FORMAT = "{0}/{1}/{2}.{3}";
        private const string ENTITY_WITH_PARENT_URL_FORMAT = "{0}/{1}/{2}/{3}.{4}";
        private const string WIKI_INDEX_URL_FORMAT = "{0}/projects/{1}/wiki/index.{2}";
        private const string WIKI_PAGE_URL_FORMAT = "{0}/projects/{1}/wiki/{2}.{3}";
        private const string WIKI_VERSION_URL_FORMAT = "{0}/projects/{1}/wiki/{2}/{3}.{4}";
        private const string ATTACHMENT_UPDATE_URL_FORMAT = "{0}/attachments/issues/{1}.{2}";


        /// <summary>
        ///     Gets the upload URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="obj">The object.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public IUrlBuild UploadUrl<T>(string id, T obj, string projectId = null)
            where T : class, new()
        {
            var type = typeof(T);

            if (!typePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[type], id, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        ///     Gets the create URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        ///     The owner id(project id) is mandatory!
        ///     or
        ///     The owner id(issue id) is mandatory!
        /// </exception>
        public IUrlBuild CreateUrl<T>(string ownerId) where T : class, new()
        {
            var type = typeof(T);

            if (!typePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(project id) is mandatory!");
                url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.PROJECTS,
                    ownerId, typePath[type], mimeRepresentation[MimeType]);
            }
            else
            {
                if (type == typeof(IssueRelation))
                {
                    if (string.IsNullOrEmpty(ownerId))
                        throw new RedmineException("The owner id(issue id) is mandatory!");
                    url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.ISSUES,
                        ownerId, typePath[type], mimeRepresentation[MimeType]);
                }
                else
                {
                    url = string.Format(URL_FORMAT, Host, typePath[type],
                        mimeRepresentation[MimeType]);
                }
            }
            return this;
        }

        /// <summary>
        /// Gets the delete URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public IUrlBuild DeleteUrl<T>(string id) where T : class, new()
        {
            var type = typeof(T);

            if (!typePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[type], id, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the get URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public IUrlBuild GetUrl<T>(string id) where T : class, new()
        {
            var type = typeof(T);

            if (!typePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[type], id, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the list URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        /// The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.
        /// or
        /// The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters
        /// </exception>
        public IUrlBuild ItemsUrl<T>(NameValueCollection parameters)
            where T : class, new()
        {
            var type = typeof(T);

            if (!typePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                var projectId = parameters.GetValue(RedmineKeys.PROJECT_ID);
                if (string.IsNullOrEmpty(projectId))
                    throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");

                url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.PROJECTS, projectId, typePath[type], mimeRepresentation[MimeType]);
            }
            else
            {
                if (type == typeof(IssueRelation))
                {
                    var issueId = parameters.GetValue(RedmineKeys.ISSUE_ID);
                    if (string.IsNullOrEmpty(issueId))
                        throw new RedmineException("The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters");

                    url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.ISSUES, issueId, typePath[type], mimeRepresentation[MimeType]);
                }
                else
                {
                    url = string.Format(URL_FORMAT, Host, typePath[type], mimeRepresentation[MimeType]);
                }
            }
            return this;
        }

        /// <summary>
        /// Gets the wikis URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public IUrlBuild WikisUrl(string projectId)
        {
            url = string.Format(WIKI_INDEX_URL_FORMAT, Host, projectId, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the wiki page URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public IUrlBuild WikiPageUrl(string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            url = version == 0
               ? string.Format(WIKI_PAGE_URL_FORMAT, Host, projectId, pageName, mimeRepresentation[MimeType])
               : string.Format(WIKI_VERSION_URL_FORMAT, Host, projectId, pageName, version, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the add user to group URL.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns></returns>
        public IUrlBuild AddUserToGroupUrl(int groupId)
        {
            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[typeof(Group)], $"{groupId}/users", mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the remove user from group URL.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IUrlBuild RemoveUserFromGroupUrl(int groupId, int userId)
        {
            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[typeof(Group)], $"{groupId}/users/{userId}", mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the upload file URL.
        /// </summary>
        /// <returns></returns>
        public IUrlBuild UploadFileUrl()
        {
            url = string.Format(URL_FORMAT, Host, RedmineKeys.UPLOADS, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the current user URL.
        /// </summary>
        /// <returns></returns>
        public IUrlBuild CurrentUserUrl()
        {
            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[typeof(User)], "current", mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the wiki create or updater URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public IUrlBuild WikiCreateOrUpdaterUrl(string projectId, string pageName)
        {
            url = string.Format(WIKI_PAGE_URL_FORMAT, Host, projectId, pageName, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the delete wikir URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public IUrlBuild DeleteWikirUrl(string projectId, string pageName)
        {
            url = string.Format(WIKI_PAGE_URL_FORMAT, Host, projectId, pageName, mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the add watcher URL.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IUrlBuild AddWatcherToIssueUrl(int issueId, int userId)
        {
            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[typeof(Issue)], $"{issueId}/watchers", mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the remove watcher URL.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IUrlBuild RemoveWatcherFromIssueUrl(int issueId, int userId)
        {
            url = string.Format(REQUEST_URL_FORMAT, Host, typePath[typeof(Issue)], $"{issueId}/watchers/{userId}", mimeRepresentation[MimeType]);
            return this;
        }

        /// <summary>
        /// Gets the attachment update URL.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <returns></returns>
        public IUrlBuild AttachmentUpdateUrl(int issueId)
        {
            url = string.Format(ATTACHMENT_UPDATE_URL_FORMAT, Host, issueId, mimeRepresentation[MimeType]);
            return this;
        }

        private NameValueCollection parameters;

        public IUrlBuild SetParameters(NameValueCollection parameters)
        {
            this.parameters = parameters;

            return this;
        }

        public string Build()
        {
            bool questionMarkSet = false;
            if (parameters != null)
            {
                var array = (from key in parameters.AllKeys
                             from value in parameters.GetValues(key)
                             select string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value)));

                url += "?" + string.Join("&", array);
                questionMarkSet = true;
            }

            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                return $"{url}{(questionMarkSet ? "&" : "?")}key={ ApiKey}";
            }
            return url;
        }
    }
}