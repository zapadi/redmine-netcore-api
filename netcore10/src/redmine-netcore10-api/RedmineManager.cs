using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

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
        public static IImmutableDictionary<Type,string> TypePath => typePath.ToImmutableDictionary();
        public static IImmutableDictionary<MimeType, string> MimeRepresentation => mimeRepresentation.ToImmutableDictionary();

        internal string ApiKey { get; }

        internal static Dictionary<Type, string> typePath = new Dictionary<Type, string>
        {
            [typeof(Issue)]= "issues",
            [typeof(Project)]= "projects",
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

        public RedmineManager(string host, string apiKey, MimeType mimeType = MimeType.Json)
        {
            Extensions.CheckIfHostIsValid(host);
            Host = host;
            ApiKey = apiKey;
            MimeType = mimeType;
        }



        //public async Task<bool> Method1()
        //{
        //    try
        //    {
        //        var uri = $"{Host}/projects/92.{mimeRepresentation[MimeType]}?apiKey={ApiKey}";
        //        await RedmineHttp.Get<Project>(new Uri(uri));
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public async Task<T> Create<T>()
        {
            throw new NotImplementedException($"{nameof(Create)}");
        }

        public async Task<T> Get<T>(string id, NameValueCollection parameters) where T : class, new()
        {
            var uri = $"{UrlHelper.GetGetUrl<T>(this, id)}?apiKey={ApiKey}";
            var response = await RedmineHttp.Get<T>(new Uri(uri), MimeType);

            return response;
        }

        public async Task<T> Update<T>()
        {
            throw new NotImplementedException($"{nameof(Update)}");
        }

        public async Task<T> Delete<T>()
        {
            throw new NotImplementedException($"{nameof(Delete)}");
        }
    }
}