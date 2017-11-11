using System.Collections.Generic;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Types;
using Xunit;

namespace RedmineApi.Core.UnitTests
{
    public class IssueUnitTests
    {
        [Fact]
        public void Should_Deserialize_List_OfType_Issue_From_Json()
        {
            const string input = "{\"issues\":[{\"id\":116,\"project\":{\"id\":92,\"name\":\"Test\"},\"tracker\":{\"id\":5,\"name\":\"Test project tracker\"},\"status\":{\"id\":5,\"name\":\"Bug\"},\"priority\":{\"id\":9,\"name\":\"Normal\"},\"author\":{\"id\":8,\"name\":\"Alina Chitu\"},\"subject\":\"Mannually test\",\"description\":\"Description\",\"start_date\":\"2016-09-30\",\"done_ratio\":0,\"created_on\":\"2016-09-30T09:07:53Z\",\"updated_on\":\"2016-09-30T09:07:53Z\"},{\"id\":115,\"project\":{\"id\":92,\"name\":\"Test\"},\"tracker\":{\"id\":5,\"name\":\"Test project tracker\"},\"status\":{\"id\":5,\"name\":\"Bug\"},\"priority\":{\"id\":9,\"name\":\"Normal\"},\"author\":{\"id\":8,\"name\":\"Alina Chitu\"},\"subject\":\"Add support for .net core\",\"description\":\"Test\",\"start_date\":\"2016-09-30\",\"done_ratio\":0,\"created_on\":\"2016-09-30T09:04:34Z\",\"updated_on\":\"2016-09-30T09:07:31Z\"},{\"id\":114,\"project\":{\"id\":92,\"name\":\"Test\"},\"tracker\":{\"id\":5,\"name\":\"Test project tracker\"},\"status\":{\"id\":5,\"name\":\"Bug\"},\"priority\":{\"id\":9,\"name\":\"Normal\"},\"author\":{\"id\":8,\"name\":\"Alina Chitu\"},\"subject\":\"Test\",\"description\":\"Descriere de test\",\"start_date\":\"2016-09-29\",\"done_ratio\":0,\"created_on\":\"2016-09-29T18:01:59Z\",\"updated_on\":\"2016-09-29T18:01:59Z\"},{\"id\":113,\"project\":{\"id\":92,\"name\":\"Test\"},\"tracker\":{\"id\":5,\"name\":\"Test project tracker\"},\"status\":{\"id\":5,\"name\":\"Bug\"},\"priority\":{\"id\":9,\"name\":\"Normal\"},\"author\":{\"id\":8,\"name\":\"Alina Chitu\"},\"category\":{\"id\":19,\"name\":\"Issue category 1\"},\"subject\":\"Test\",\"description\":\"\",\"start_date\":\"2016-09-06\",\"done_ratio\":0,\"created_on\":\"2016-09-06T19:24:36Z\",\"updated_on\":\"2016-09-08T16:59:22Z\"}],\"total_count\":4,\"offset\":0,\"limit\":25}";

            var result = RedmineSerializer.DeserializeList<Issue>(input, MimeType.Json);
            Assert.NotNull(result.Items);
            Assert.NotEmpty(result.Items);
			Assert.True(result.Items.Count == 4);
            Assert.True(result.Offset == 0);
            Assert.True(result.Total == 4);
            Assert.True(result.Limit == 25);

        }

        [Fact]
        public void Should_Serialize_List_Of_CustomField_To_Json()
        {
			Issue issue = new Issue();
			issue.CustomFields = new List<IssueCustomField>()
			{
                CreateIssueCustomField(1,"Affected version",false, "1.0.1"),
				CreateIssueCustomField(2, "Resolution", false, "Fixed" ),
			};

            var actual = RedmineSerializer.Serialize(issue, MimeType.Json);


        }

        [Fact]
        public void Should_Serialize_List_Of_CustomField_With_MultiSelect_To_Json()
        {
            Issue issue = new Issue();
            issue.CustomFields = new List<IssueCustomField>()
            {
                CreateIssueCustomField(1,"Affected version",true, "1.0.1", "1.0.2"),
                CreateIssueCustomField(2, "Resolution", false, "Fixed" ),
            };

            var actual = RedmineSerializer.Serialize(issue, MimeType.Json);
        }

        private IssueCustomField CreateIssueCustomField(int id, string name, bool multiple, params string[] values)
        {
            var icf = new IssueCustomField();

            icf.Id = id;
            icf.Name = name;
            icf.Multiple = multiple;

            icf.Values = new List<CustomFieldValue>();

            foreach (var value in values)
            {
                icf.Values.Add(new CustomFieldValue(){Info = value});
            }

            return icf;
        }

		[Fact]
		public void Should_Deserialize_List_Of_CustomField_From_Json()
		{
            const string response = "{\"issue\":{\"id\":8471,\"custom_fields\":[{\"value\":[\"1.0.1\",\"1.0.2\"],\"multiple\":true,\"name\":\"Affected version\",\"id\":1},{\"value\":\"Fixed\",\"name\":\"Resolution\",\"id\":2}]}}";

            var actual = RedmineSerializer.Deserialize<Issue>(response, MimeType.Json);

            Assert.NotNull(actual);
            Assert.True(actual.Id == 8471);
            Assert.NotNull(actual.CustomFields);
            Assert.NotEmpty(actual.CustomFields);

		}
    }


}