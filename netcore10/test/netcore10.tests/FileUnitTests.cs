using System;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Types;
using Xunit;

namespace RedmineApi.Core.UnitTests
{
    public sealed class FileUnitTests
    {
        [Fact]
        public void Should_Deserialize_TypeOf_File_From_Json()
        {
            const string INPUT = @"{
            ""file"":{
                ""id"":12,
                ""filename"": ""foo-1.0-setup.exe"",
                ""filesize"": 74753799,
                ""content_type"": ""application/octet-stream"",
                ""description"":""Foo App for Windows"",
                ""content_url"": ""http://localhost:3000/attachments/download/12/foo-1.0-setup.exe"",
                ""author"":{ ""id"":1, ""name"": ""Redmine Admin""},
                ""created_on"": ""2017-01-04T09:12:32Z"",
                ""version"":{ ""id"":2, ""name"":""1.0""},
                ""digest"":""1276481102f218c981e0324180bafd9f"",
                ""downloads"":12
                }
            }";

            var expected = new File
            {
                Id = 12,
                Filesize = 74753799,
                ContentType = "application/octet-stream",
                ContentUrl = "http://localhost:3000/attachments/download/12/foo-1.0-setup.exe",
                Author = new IdentifiableName {Id = 1, Name = "Redmine Admin"},
                CreatedOn = new DateTime(2017, 01, 04, 9, 12, 32, DateTimeKind.Utc),
                Digest = "1276481102f218c981e0324180bafd9f",
                Downloads = 12,
                Version = new IdentifiableName {Id = 2, Name = "1.0"},
                Filename = "foo-1.0-setup.exe",
                Description = "Foo App for Windows"
            };

            var actual = RedmineSerializer.Deserialize<File>(INPUT, MimeType.Json);
            Assert.True(actual.GetHashCode() == expected.GetHashCode(), "File deserialize error.");
        }

        [Fact]
        public void Should_Serialize_TypeOf_File_To_Json()
        {
            const string EXPECTED = @"{
  ""file"": {
    ""token"": ""21.01a1d7b1c2ffcbbc9ecf14debeec27d8"",
    ""version_id"": 2,
    ""filename"": ""foo-1.0-src.tar.tgz"",
    ""description"": ""Foo App source code""
  }
}";

            var file = new File
            {
                Token = "21.01a1d7b1c2ffcbbc9ecf14debeec27d8",
                Version = new IdentifiableName {Id = 2},
                Filename = "foo-1.0-src.tar.tgz",
                Description = "Foo App source code"
            };

            var actual = RedmineSerializer.Serialize(file, MimeType.Json);

            Assert.True(actual.Equals(EXPECTED), "File type serialization failed.");
        }

        [Fact]
        public void ShouldDeserializeFileXml()
        {
            const string RESPONSE = @"
            <file>
                <id>12</id>
                <filename>foo-1.0-setup.exe</filename>
                <filesize>74753799</filesize>
                <content_type>application/octet-stream</content_type>
                <description>Foo App for Windows</description>
                <content_url>http://localhost:3000/attachments/download/12/foo-1.0-setup.exe</content_url>
                <author id=""1"" name=""Redmine Admin""/>
                <created_on>2017-01-04T09:12:32Z</created_on>
                <version id=""2"" name=""1.0""/>
                <digest>1276481102f218c981e0324180bafd9f</digest>
                <downloads>12</downloads>
            </file>";

            var expected = new File
            {
                Id = 12,
                Filesize = 74753799,
                ContentType = "application/octet-stream",
                ContentUrl = "http://localhost:3000/attachments/download/12/foo-1.0-setup.exe",
                Author = new IdentifiableName {Id = 1, Name = "Redmine Admin"},
                CreatedOn = new DateTime(2017, 01, 04, 11, 12, 32, DateTimeKind.Utc),
                Digest = "1276481102f218c981e0324180bafd9f",
                Downloads = 12,
                Version = new IdentifiableName {Id = 2, Name = "1.0"},
                Filename = "foo-1.0-setup.exe",
                Description = "Foo App for Windows"
            };

            var actual = RedmineSerializer.Deserialize<File>(RESPONSE, MimeType.Xml);
            Assert.True(actual.GetHashCode() == expected.GetHashCode(), "File deserialize error.");
        }

        [Fact]
        public void ShouldDeserializePartialFileXml()
        {
            const string RESPONSE = @"
            <file>
              <token>21.01a1d7b1c2ffcbbc9ecf14debeec27d8</token>
              <version_id>2</version_id>
              <filename>foo-1.0-src.tar.tgz</filename>
              <description>Foo App source code</description>
            </file>";

            var expected = new File
            {
                Token = "21.01a1d7b1c2ffcbbc9ecf14debeec27d8",
                Version = new IdentifiableName {Id = 2},
                Filename = "foo-1.0-src.tar.tgz",
                Description = "Foo App source code"
            };

            var actual = RedmineSerializer.Deserialize<File>(RESPONSE, MimeType.Xml);
            Assert.True(actual.Equals(expected), "File deserialize error.");
        }

        [Fact]
        public void ShouldSerializeFileXml()
        {
            const string EXPECTED = @"<file><token>21.01a1d7b1c2ffcbbc9ecf14debeec27d8</token><version_id>2</version_id><filename>foo-1.0-src.tar.tgz</filename><description>Foo App source code</description></file>";

            var file = new File
            {
                Token = "21.01a1d7b1c2ffcbbc9ecf14debeec27d8",
                Version = new IdentifiableName {Id = 2},
                Filename = "foo-1.0-src.tar.tgz",
                Description = "Foo App source code"
            };

            var actual = RedmineSerializer.Serialize(file, MimeType.Xml);

            Assert.True(actual.Equals(EXPECTED), "File type serialization failed.");
        }
    }
}