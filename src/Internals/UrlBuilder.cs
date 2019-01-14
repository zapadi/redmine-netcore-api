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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Extensions;
using RedmineApi.Core.Types;
using Version = RedmineApi.Core.Types.Version;

namespace RedmineApi.Core.Internals
{
    /// <summary>
    /// </summary>
    internal sealed class UrlBuilder : IUrlBuild
    {
        private const string URL_FORMAT = "{0}/{1}.{2}";
        private const string REQUEST_URL_FORMAT = "{0}/{1}/{2}.{3}";
        private const string ENTITY_WITH_PARENT_URL_FORMAT = "{0}/{1}/{2}/{3}.{4}";
        private const string WIKI_INDEX_URL_FORMAT = "{0}/projects/{1}/wiki/index.{2}";
        private const string WIKI_PAGE_URL_FORMAT = "{0}/projects/{1}/wiki/{2}.{3}";
        private const string WIKI_VERSION_URL_FORMAT = "{0}/projects/{1}/wiki/{2}/{3}.{4}";
        private const string ATTACHMENT_UPDATE_URL_FORMAT = "{0}/attachments/issues/{1}.{2}";
        private const string FILE_URL_FORMAT = "{0}/projects/{1}/files.{2}";
        private const string ADD_USER_TO_GROUP_FORMAT = "{0}/{1}/{2}/users.{3}";
        private const string REMOVE_USER_FROM_GROUP_FORMAT = "{0}/{1}/{2}/users/{3}.{4}";
        private const string ADD_WATCHER_TO_ISSUE_FORMAT = "{0}/{1}/{2}/watchers.{3}";
        private const string REMOVE_WATCHER_FROM_ISSUE_FORMAT = "{0}/{1}/{2}/watchers/{3}.{4}";

        private string url;
        private string host;
        private string mimeType;

        private UrlBuilder()
        {
        }

        private NameValueCollection parameters;

        public IUrlBuild SetParameters(NameValueCollection parameters)
        {
            this.parameters = parameters;
            return this;
        }

        public string Build()
        {
            if (parameters != null)
            {
                var array = from key in parameters.AllKeys
                            from value in parameters.GetValues(key)
                            select string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value));

                url += "?" + string.Join("&", array);
            }

            return url;
        }

        public static UrlBuilder Create(string host, MimeType mimeType)
        {
            return new UrlBuilder {host = host, mimeType = RedmineMimeTypeHelper.GetStringRepresentation(mimeType)};
        }

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
            var path = RedmineTypes.GetPath<T>();

            url = string.Format(REQUEST_URL_FORMAT, host, path, id, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the create URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="RedmineApi.Core.Exceptions.RedmineException">
        ///     The owner id(project id) is mandatory!
        ///     or
        ///     The owner id(issue id) is mandatory!
        /// </exception>
        public IUrlBuild CreateUrl<T>(string ownerId) where T : class, new()
        {
            var type = typeof(T);

            var path = RedmineTypes.GetPath(type);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                EnsureIdIsDefined(ownerId, "project");

                url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, host, RedmineKeys.PROJECTS, ownerId, path, mimeType);
            }
            else
            {
                if (type == typeof(IssueRelation))
                {
                    EnsureIdIsDefined(ownerId, "issue");

                    url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, host, RedmineKeys.ISSUES, ownerId, path, mimeType);
                }
                else
                {
                    if (type == typeof(File))
                    {
                        EnsureIdIsDefined(ownerId, "project");

                        url = string.Format(FILE_URL_FORMAT, host, ownerId, mimeType);
                    }
                    else
                    {
                        url = string.Format(URL_FORMAT, host, path, mimeType);
                    }
                }
            }

            return this;
        }

        /// <summary>
        ///     Gets the delete URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="reassignedId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public IUrlBuild DeleteUrl<T>(string id, string reassignedId) where T : class, new()
        {
            var path = RedmineTypes.GetPath<T>();

            url = string.Format(REQUEST_URL_FORMAT, host, path, id, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the get URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public IUrlBuild GetUrl<T>(string id) where T : class, new()
        {
            var path = RedmineTypes.GetPath<T>();

            url = string.Format(REQUEST_URL_FORMAT, host, path, id, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the list URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="RedmineApi.Core.Exceptions.RedmineException">
        ///     The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.
        ///     or
        ///     The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters
        /// </exception>
        public IUrlBuild ItemsUrl<T>(NameValueCollection parameters)
            where T : class, new()
        {
            var type = typeof(T);

            var path = RedmineTypes.GetPath(type);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                var projectId = parameters.GetValue(RedmineKeys.PROJECT_ID);
                if (string.IsNullOrEmpty(projectId))
                {
                    throw new RedmineException(
                        "The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");
                }

                url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, host, RedmineKeys.PROJECTS, projectId, path, mimeType);
            }
            else
            {
                if (type == typeof(IssueRelation))
                {
                    var issueId = parameters.GetValue(RedmineKeys.ISSUE_ID);
                    if (string.IsNullOrEmpty(issueId))
                    {
                        throw new RedmineException(
                            "The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters");
                    }

                    url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, host, RedmineKeys.ISSUES, issueId, path, mimeType);
                }
                else
                {
                    if (type == typeof(File))
                    {
                        var projectId = parameters.GetValue(RedmineKeys.PROJECT_ID);
                        if (string.IsNullOrEmpty(projectId))
                        {
                            throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");
                        }

                        url = string.Format(FILE_URL_FORMAT, host, projectId, mimeType);
                    }
                    else
                    {
                        url = string.Format(URL_FORMAT, host, path, mimeType);
                    }
                }
            }

            return this;
        }

        /// <summary>
        ///     Gets the wikis URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public IUrlBuild WikiUrl(string projectId)
        {
            url = string.Format(WIKI_INDEX_URL_FORMAT, host, projectId, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the wiki page URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public IUrlBuild WikiPageUrl(string projectId, string pageName, uint version = 0)
        {
            url = version == 0
                ? string.Format(WIKI_PAGE_URL_FORMAT, host, projectId, pageName, mimeType)
                : string.Format(WIKI_VERSION_URL_FORMAT, host, projectId, pageName, version, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the add user to group URL.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns></returns>
        public IUrlBuild AddUserToGroupUrl(int groupId)
        {
            var path = RedmineTypes.GetPath<Group>();

            url = string.Format(ADD_USER_TO_GROUP_FORMAT, host, path, groupId, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the remove user from group URL.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IUrlBuild RemoveUserFromGroupUrl(int groupId, int userId)
        {
            var path = RedmineTypes.GetPath<Group>();

            url = string.Format(REMOVE_USER_FROM_GROUP_FORMAT, host, path, groupId, userId, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the upload file URL.
        /// </summary>
        /// <returns></returns>
        public IUrlBuild UploadFileUrl()
        {
            url = string.Format(URL_FORMAT, host, RedmineKeys.UPLOADS, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the current user URL.
        /// </summary>
        /// <returns></returns>
        public IUrlBuild CurrentUserUrl()
        {
            var path = RedmineTypes.GetPath<User>();
            url = string.Format(REQUEST_URL_FORMAT, host, path, RedmineKeys.CURRENT_USER, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the wiki create or updater URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public IUrlBuild WikiCreateOrUpdaterUrl(string projectId, string pageName)
        {
            url = string.Format(WIKI_PAGE_URL_FORMAT, host, projectId, pageName, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the delete wiki URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public IUrlBuild DeleteWikiUrl(string projectId, string pageName)
        {
            url = string.Format(WIKI_PAGE_URL_FORMAT, host, projectId, pageName, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the add watcher URL.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IUrlBuild AddWatcherToIssueUrl(int issueId, int userId)
        {
            var path = RedmineTypes.GetPath<Issue>();
            url = string.Format(ADD_WATCHER_TO_ISSUE_FORMAT, host, path, issueId, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the remove watcher URL.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IUrlBuild RemoveWatcherFromIssueUrl(int issueId, int userId)
        {
            var path = RedmineTypes.GetPath<Issue>();
            url = string.Format(REMOVE_WATCHER_FROM_ISSUE_FORMAT, host, path, issueId, userId, mimeType);
            return this;
        }

        /// <summary>
        ///     Gets the attachment update URL.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <returns></returns>
        public IUrlBuild AttachmentUpdateUrl(int issueId)
        {
            url = string.Format(ATTACHMENT_UPDATE_URL_FORMAT, host, issueId, mimeType);
            return this;
        }

        private static void EnsureIdIsDefined(string id, string type)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new RedmineException($"The {type} id is mandatory!");
            }
        }
    }
}