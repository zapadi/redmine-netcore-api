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

        private UrlBuilder()
        {
        }

        private NameValueCollection Parameters { get; set; }

        private string Url { get; set; }

        private string Host { get; set; }
        private MimeType MimeType { get; set; }

        public IUrlBuild SetParameters(NameValueCollection parameters)
        {
            Parameters = parameters;
            return this;
        }

        public string Build()
        {
            if (Parameters != null)
            {
                var array = from key in Parameters.AllKeys
                    from value in Parameters.GetValues(key)
                    select string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value));

                Url += "?" + string.Join("&", array);
            }

            return Url;
        }

        public static UrlBuilder Create(string host, MimeType mimeType)
        {
            return new UrlBuilder {Host = host, MimeType = mimeType};
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

            Url = string.Format(REQUEST_URL_FORMAT, Host, path, id, Mime.GetStringRepresentation(MimeType));
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
                if (string.IsNullOrEmpty(ownerId))
                {
                    throw new RedmineException("The owner id(project id) is mandatory!");
                }

                Url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.PROJECTS, ownerId, path, Mime.GetStringRepresentation(MimeType));
            }
            else
            {
                if (type == typeof(IssueRelation))
                {
                    if (string.IsNullOrEmpty(ownerId))
                    {
                        throw new RedmineException("The owner id(issue id) is mandatory!");
                    }

                    Url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.ISSUES, ownerId, path, Mime.GetStringRepresentation(MimeType));
                }
                else
                {
                    if (type == typeof(File))
                    {
                        if (string.IsNullOrEmpty(ownerId))
                        {
                            throw new RedmineException("The owner id(project id) is mandatory!");
                        }
                        Url = string.Format(FILE_URL_FORMAT, Host, ownerId, Mime.GetStringRepresentation(MimeType));
                    }
                    else
                    {
                        Url = string.Format(URL_FORMAT, Host, path, Mime.GetStringRepresentation(MimeType));
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

            Url = string.Format(REQUEST_URL_FORMAT, Host, path, id, Mime.GetStringRepresentation(MimeType));
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

            Url = string.Format(REQUEST_URL_FORMAT, Host, path, id, Mime.GetStringRepresentation(MimeType));
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

                Url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.PROJECTS, projectId, path, Mime.GetStringRepresentation(MimeType));
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

                    Url = string.Format(ENTITY_WITH_PARENT_URL_FORMAT, Host, RedmineKeys.ISSUES, issueId, path, Mime.GetStringRepresentation(MimeType));
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

                        Url = string.Format(FILE_URL_FORMAT, Host, projectId, Mime.GetStringRepresentation(MimeType));
                    }
                    else
                    {
                        Url = string.Format(URL_FORMAT, Host, path, Mime.GetStringRepresentation(MimeType));
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
            Url = string.Format(WIKI_INDEX_URL_FORMAT, Host, projectId, Mime.GetStringRepresentation(MimeType));
            return this;
        }

        /// <summary>
        ///     Gets the wiki page URL.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public IUrlBuild WikiPageUrl(string projectId, NameValueCollection parameters, string pageName,
            uint version = 0)
        {
            Url = version == 0
                ? string.Format(WIKI_PAGE_URL_FORMAT, Host, projectId, pageName, Mime.GetStringRepresentation(MimeType))
                : string.Format(WIKI_VERSION_URL_FORMAT, Host, projectId, pageName, version, Mime.GetStringRepresentation(MimeType));
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

            Url = string.Format(REQUEST_URL_FORMAT, Host, path, $"{groupId}/users", Mime.GetStringRepresentation(MimeType));
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

            Url = string.Format(REQUEST_URL_FORMAT, Host, path, $"{groupId}/users/{userId}", Mime.GetStringRepresentation(MimeType));
            return this;
        }

        /// <summary>
        ///     Gets the upload file URL.
        /// </summary>
        /// <returns></returns>
        public IUrlBuild UploadFileUrl()
        {
            Url = string.Format(URL_FORMAT, Host, RedmineKeys.UPLOADS, Mime.GetStringRepresentation(MimeType));
            return this;
        }

        /// <summary>
        ///     Gets the current user URL.
        /// </summary>
        /// <returns></returns>
        public IUrlBuild CurrentUserUrl()
        {
            var path = RedmineTypes.GetPath<User>();
            Url = string.Format(REQUEST_URL_FORMAT, Host, path, "current", Mime.GetStringRepresentation(MimeType));
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
            Url = string.Format(WIKI_PAGE_URL_FORMAT, Host, projectId, pageName, Mime.GetStringRepresentation(MimeType));
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
            Url = string.Format(WIKI_PAGE_URL_FORMAT, Host, projectId, pageName, Mime.GetStringRepresentation(MimeType));
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
            Url = string.Format(REQUEST_URL_FORMAT, Host, path, $"{issueId}/watchers", Mime.GetStringRepresentation(MimeType));
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
            Url = string.Format(REQUEST_URL_FORMAT, Host, path, $"{issueId}/watchers/{userId}", Mime.GetStringRepresentation(MimeType));
            return this;
        }

        /// <summary>
        ///     Gets the attachment update URL.
        /// </summary>
        /// <param name="issueId">The issue identifier.</param>
        /// <returns></returns>
        public IUrlBuild AttachmentUpdateUrl(int issueId)
        {
            Url = string.Format(ATTACHMENT_UPDATE_URL_FORMAT, Host, issueId, Mime.GetStringRepresentation(MimeType));
            return this;
        }
    }
}