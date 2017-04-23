using Xunit;

namespace Tests
{
    [CollectionDefinition(Keywords.REDMINE_MANAGER_COLLECTION)]
    public class RedmineManagerCollection : ICollectionFixture<RedmineManagerFixture>
    {
    }
}