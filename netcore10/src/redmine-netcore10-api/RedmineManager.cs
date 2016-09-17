using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Redmine.Net.Api;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Group = Redmine.Net.Api.Types.Group;

namespace ClassLibrary
{
    public class RedmineManager
    {
        static readonly Dictionary<MimeType, string> mimeRepresentation = new Dictionary<MimeType, string>
        {
            [MimeType.Json] = MimeType.Json.ToString().ToLowerInvariant(),
            [MimeType.Xml] = MimeType.Xml.ToString().ToLowerInvariant()
        };

        public MimeType MimeType { get; }
        public string Host { get; }
        internal string ApiKey { get; }

        public static IImmutableDictionary<Type, string> TypePath => typePath.ToImmutableDictionary();
        public static IImmutableDictionary<MimeType, string> MimeRepresentation => mimeRepresentation.ToImmutableDictionary();

        internal static Dictionary<Type, string> typePath = new Dictionary<Type, string>
        {
            [typeof(Issue)] = "issues",
            [typeof(Project)] = "projects",
            [typeof(User)] = "users",
            [typeof(News)] = "news",
            [typeof(Query)] = "queries",
            [typeof(Redmine.Net.Api.Types.Version)] = "versions",
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

        public RedmineManager(string host, string apiKey, MimeType mimeType = MimeType.Xml)
        {
            Extensions.CheckIfHostIsValid(host);
            Host = host;
            ApiKey = apiKey;
            MimeType = mimeType;
        }

        ~RedmineManager()
        {
            Close();
        }

        public void Close()
        {
            RedmineHttp.Dispose();
        }

        public async Task<TData> Create<TData>(TData data)
            where TData : class, new()
        {
            return await Create(null, data);
        }

        public async Task<TData> Create<TData>(string ownerId, TData data) 
            where TData : class, new()
        {
            var uri = $"{UrlBuilder.CreateUrl<TData>(this, ownerId)}?key={ApiKey}";
            var response = await RedmineHttp.Post(new Uri(uri), data, MimeType);

            return response;
        }

        public async Task<TData> Get<TData>(string id, NameValueCollection parameters) 
            where TData : class, new()
        {
            var uri = $"{UrlBuilder.GetUrl<TData>(this, id)}?key={ApiKey}";
            var response = await RedmineHttp.Get<TData>(new Uri(uri), MimeType);

            return response;
        }

        public async Task<TData> Get<TData>(string url)
           where TData : class, new()
        {
            var uri = $"{url}?key={ApiKey}";
            var response = await RedmineHttp.Get<TData>(new Uri(uri), MimeType);

            return response;
        }

        public async Task<List<TData>> List<TData>(int limit, int offset, params string[] include) 
            where TData : class, new()
        {
            throw new NotImplementedException("Get list");
        }

        public async Task<PaginatedResult<TData>> List<TData>(NameValueCollection parameters) 
            where TData : class, new()
        {
            throw new NotImplementedException("Get list");
        }

        public async Task<TData> Update<TData>(string id, TData data) where TData : class, new()
        {
            return await Update(id, data, null);
        }

        public async Task<TData> Update<TData>(string id, TData data, string projectId) where TData : class, new()
        {
            var uri = $"{UrlBuilder.UploadUrl<TData>(this, id, data)}?key={ApiKey}";
            
            var response = await RedmineHttp.Put<TData>(new Uri(uri), data, MimeType).ConfigureAwait(false);

            return response;
        }

        public async Task<HttpStatusCode> Delete<T>(string id) where T : class, new()
        {
            return await Delete<T>(id, null);
        }

        public async Task<HttpStatusCode> Delete<T>(string id, string reasignedId) where T : class, new()
        {
            var uri = $"{UrlBuilder.DeleteUrl<T>(this, id)}?key={ApiKey}";
            var response = await RedmineHttp.Delete(new Uri(uri), MimeType).ConfigureAwait(false);

            return response;
        }
    }
}