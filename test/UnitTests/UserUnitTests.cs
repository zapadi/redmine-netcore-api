using RedmineApi.Core.Types;
using Xunit;

namespace RedmineApi.Core.UnitTests
{
    public class UserUnitTests
    {
        [Fact]
        public void Unprocessable_Entity_Response_with_the_error_messages_in_its_body()
        {
            const string response = "{\"errors\":[\"First name can't be blank\",\"Email is invalid\"]}";

           // var actual = RedmineApi.Core.Serializers.RedmineSerializer.Deserialize<Error>(response, MimeType.Json);
        }
    }
}