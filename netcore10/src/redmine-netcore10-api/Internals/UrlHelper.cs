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

using System.Collections.Generic;
using Redmine.Net.Api.Types;
using ClassLibrary;
using System.Collections.Specialized;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// </summary>
    internal static class UrlBuilder
    {
        /// <summary>
        /// </summary>
        private const string REQUEST_URL_FORMAT = "{0}/{1}/{2}.{3}";

        /// <summary>
        /// </summary>
        private const string URL_FORMAT = "{0}/{1}.{2}";

        /// <summary>
        /// </summary>
        private const string WIKI_INDEX_URL_FORMAT = "{0}/projects/{1}/wiki/index.{2}";

        /// <summary>
        /// </summary>
        private const string WIKI_PAGE_URL_FORMAT = "{0}/projects/{1}/wiki/{2}.{3}";

        /// <summary>
        /// </summary>
        private const string WIKI_VERSION_URL_FORMAT = "{0}/projects/{1}/wiki/{2}/{3}.{4}";

        /// <summary>
        /// </summary>
        private const string ENTITY_WITH_PARENT_URL_FORMAT = "{0}/{1}/{2}/{3}.{4}";

        /// <summary>
        /// </summary>
        private const string ATTACHMENT_UPDATE_URL_FORMAT = "{0}/attachments/issues/{1}.{2}";

        /// <summary>
        /// </summary>
        private const string CURRENT_USER_URI = "current";
        /// <summary>
        ///     Gets the upload URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="obj">The object.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public static string UploadUrl<T>(RedmineManager redmineManager, string id, T obj, string projectId = null)
            where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.TypePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host, RedmineManager.TypePath[type], id,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        ///     Gets the create URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        ///     The owner id(project id) is mandatory!
        ///     or
        ///     The owner id(issue id) is mandatory!
        /// </exception>
        public static string CreateUrl<T>(RedmineManager redmineManager, string ownerId) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.TypePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(project id) is mandatory!");
                return string.Format(ENTITY_WITH_PARENT_URL_FORMAT, redmineManager.Host, RedmineKeys.PROJECTS,
                    ownerId, RedmineManager.TypePath[type], RedmineManager.MimeRepresentation[redmineManager.MimeType]);
            }
            if (type == typeof(IssueRelation))
            {
                if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(issue id) is mandatory!");
                return string.Format(ENTITY_WITH_PARENT_URL_FORMAT, redmineManager.Host, RedmineKeys.ISSUES,
                    ownerId, RedmineManager.TypePath[type], RedmineManager.MimeRepresentation[redmineManager.MimeType]);
            }

            return string.Format(URL_FORMAT, redmineManager.Host, RedmineManager.TypePath[type],
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the delete URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public static string DeleteUrl<T>(RedmineManager redmineManager, string id) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.TypePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host, RedmineManager.TypePath[type], id,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the get URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public static string GetUrl<T>(RedmineManager redmineManager, string id) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.TypePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host, RedmineManager.TypePath[type], id,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the list URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        /// The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.
        /// or
        /// The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters
        /// </exception>
        public static string ItemsUrl<T>(RedmineManager redmineManager, NameValueCollection parameters)
            where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.TypePath.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                var projectId = parameters.GetParameterValue(RedmineKeys.PROJECT_ID);
                if (string.IsNullOrEmpty(projectId))
                    throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");

                return string.Format(ENTITY_WITH_PARENT_URL_FORMAT, redmineManager.Host, RedmineKeys.PROJECTS,
                    projectId, RedmineManager.TypePath[type], RedmineManager.MimeRepresentation[redmineManager.MimeType]);
            }
            if (type == typeof(IssueRelation))
            {
                var issueId = parameters.GetParameterValue(RedmineKeys.ISSUE_ID);
                if (string.IsNullOrEmpty(issueId))
                    throw new RedmineException("The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters");

                return string.Format(ENTITY_WITH_PARENT_URL_FORMAT, redmineManager.Host, RedmineKeys.ISSUES,
                    issueId, RedmineManager.TypePath[type], RedmineManager.MimeRepresentation[redmineManager.MimeType]);
            }
            return string.Format(URL_FORMAT, redmineManager.Host, RedmineManager.TypePath[type],
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the wikis URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public static string WikisUrl(RedmineManager redmineManager, string projectId)
        {
            return string.Format(WIKI_INDEX_URL_FORMAT, redmineManager.Host, projectId,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the wiki page URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public static string WikiPageUrl(RedmineManager redmineManager, string projectId,
            NameValueCollection parameters, string pageName, uint version = 0)
        {
            var url = version == 0
                ? string.Format(WIKI_PAGE_URL_FORMAT, redmineManager.Host, projectId, pageName,
                    RedmineManager.MimeRepresentation[redmineManager.MimeType])
                : string.Format(WIKI_VERSION_URL_FORMAT, redmineManager.Host, projectId, pageName, version,
                    RedmineManager.MimeRepresentation[redmineManager.MimeType]);
            return url;
        }

        /// <summary>
        /// Gets the add user to group URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns></returns>
        public static string AddUserToGroupUrl(RedmineManager redmineManager, int groupId)
        {
            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host,
                RedmineManager.TypePath[typeof(Group)],
                $"{groupId}/users", RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the remove user from group URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static string RemoveUserFromGroupUrl(RedmineManager redmineManager, int groupId, int userId)
        {
            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host,
                RedmineManager.TypePath[typeof(Group)],
                $"{groupId}/users/{userId}", RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the upload file URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <returns></returns>
        public static string UploadFileUrl(RedmineManager redmineManager)
        {
            return string.Format(URL_FORMAT, redmineManager.Host, RedmineKeys.UPLOADS,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the current user URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <returns></returns>
        public static string CurrentUserUrl(RedmineManager redmineManager)
        {
            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host,
                RedmineManager.TypePath[typeof(User)], CURRENT_USER_URI,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the wiki create or updater URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public static string WikiCreateOrUpdaterUrl(RedmineManager redmineManager, string projectId, string pageName)
        {
            return string.Format(WIKI_PAGE_URL_FORMAT, redmineManager.Host, projectId, pageName,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the delete wikir URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public static string DeleteWikirUrl(RedmineManager redmineManager, string projectId, string pageName)
        {
            return string.Format(WIKI_PAGE_URL_FORMAT, redmineManager.Host, projectId, pageName,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the add watcher URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static string AddWatcherToIssueUrl(RedmineManager redmineManager, int issueId, int userId)
        {
            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host,
                RedmineManager.TypePath[typeof(Issue)], $"{issueId}/watchers",
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the remove watcher URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static string RemoveWatcherFromIssueUrl(RedmineManager redmineManager, int issueId, int userId)
        {
            return string.Format(REQUEST_URL_FORMAT, redmineManager.Host,
                RedmineManager.TypePath[typeof(Issue)], $"{issueId}/watchers/{userId}",
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }

        /// <summary>
        /// Gets the attachment update URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <returns></returns>
        public static string AttachmentUpdateUrl(RedmineManager redmineManager, int issueId)
        {
            return string.Format(ATTACHMENT_UPDATE_URL_FORMAT, redmineManager.Host, issueId,
                RedmineManager.MimeRepresentation[redmineManager.MimeType]);
        }
    }
}