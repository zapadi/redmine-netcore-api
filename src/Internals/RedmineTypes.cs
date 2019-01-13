using System;
using System.Collections.Generic;
using RedmineApi.Core.Types;
using Version = RedmineApi.Core.Types.Version;

namespace RedmineApi.Core.Internals
{
    public static class RedmineTypes
    {
        private static readonly Dictionary<Type, string> typePath = new Dictionary<Type, string>
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
            [typeof(CustomField)] = "custom_fields",
            [typeof(File)] = "files"
        };

        public static string GetPath(Type type)
        {
            if (!typePath.TryGetValue(type, out string path))
            {
                throw new KeyNotFoundException($"Type {nameof(type.Name)} is not defined.");
            }

            return path;
        }

        public static string GetPath<T>()
        {
            return GetPath(typeof(T));
        }
    }
}