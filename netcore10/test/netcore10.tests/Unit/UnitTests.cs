using RedmineApi.Core;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Types;
using System;
using Xunit;

namespace Redmine.Api.Tests
{
    [CollectionDefinition("SerializationCollection")]
    public class UnitTests
    {
        [Fact]
        public void ShouldSerializeFileXml()
        {
            const string expected = @"<file><token>21.01a1d7b1c2ffcbbc9ecf14debeec27d8</token><version_id>2</version_id><filename>foo-1.0-src.tar.tgz</filename><description>Foo App source code</description></file>";

            var file = new File
            {
                Token = "21.01a1d7b1c2ffcbbc9ecf14debeec27d8",
                Version = new IdentifiableName { Id = 2 },
                Filename = "foo-1.0-src.tar.tgz",
                Description = "Foo App source code"
            };

            var actual = RedmineSerializer.Serialize(file, MimeType.Xml);

            Assert.True(actual.Equals(expected), "File type serialization failed.");
        }

        [Fact]
        public void ShouldDeserializeFileXml()
        {
            const string response = @"
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

            var expected = new File()
            {
                Id = 12,
                Filesize = 74753799,
                ContentType = "application/octet-stream",
                ContentUrl = "http://localhost:3000/attachments/download/12/foo-1.0-setup.exe",
                Author = new IdentifiableName { Id = 1, Name = "Redmine Admin" },
                CreatedOn = new DateTime(2017, 01, 04, 11, 12, 32, DateTimeKind.Utc),
                Digest = "1276481102f218c981e0324180bafd9f",
                Downloads = 12,
                Version = new IdentifiableName { Id = 2, Name = "1.0" },
                Filename = "foo-1.0-setup.exe",
                Description = "Foo App for Windows"
            };

            var actual = RedmineSerializer.Deserialize<File>(response, MimeType.Xml);
            Assert.True(actual.GetHashCode() == expected.GetHashCode(), "File deserialize error.");
        }

        [Fact]
        public void ShouldDeserializePartialFileXml()
        {
            const string response = @"
            <file>
              <token>21.01a1d7b1c2ffcbbc9ecf14debeec27d8</token>
              <version_id>2</version_id>
              <filename>foo-1.0-src.tar.tgz</filename>
              <description>Foo App source code</description>
            </file>";

            var expected = new File
            {
                Token = "21.01a1d7b1c2ffcbbc9ecf14debeec27d8",
                Version = new IdentifiableName { Id = 2 },
                Filename = "foo-1.0-src.tar.tgz",
                Description = "Foo App source code"
            };

            var actual = RedmineSerializer.Deserialize<File>(response, MimeType.Xml);
            Assert.True(actual.Equals(expected), "File deserialize error.");
        }

        [Fact]
        public void ShouldReturnGetFileUrl()
        {
            var localhost = "http://localhost";

            var expected = $"{localhost}/projects/1/files.xml";

            var actual = UrlBuilder
                .Create(localhost, MimeType.Xml)
                .ItemsUrl<File>(new System.Collections.Specialized.NameValueCollection() { { RedmineKeys.PROJECT_ID, "1" } })
                .Build();

            Assert.True(expected == actual);
        }

        [Fact]
        public void ShouldReturnCreateFileUrl()
        {
            var localhost = "http://localhost";

            var expected = $"{localhost}/projects/1/files.xml";

            var actual = UrlBuilder
                .Create(localhost, MimeType.Xml)
                .CreateUrl<File>("1")
                .Build();

            Assert.True(expected == actual);
        }

        [Fact]
        public void ThrowExceptionWhenCreateFileUrlWithout()
        {
            var localhost = "http://localhost";

            var expected = $"{localhost}/projects/1/files.xml";

            Assert.Throws<RedmineException>(() => UrlBuilder
                .Create(localhost, MimeType.Xml)
                .CreateUrl<File>(null)
                .Build());
        }
    }
}