using System;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Serializers;
using RedmineApi.Core.Types;
using Xunit;

namespace RedmineApi.Core.UnitTests
{
    [CollectionDefinition("SerializationCollection")]
    public class UnitTests
    {
        

       

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
        public void ThrowExceptionWhenCreateFileUrlWithout_Project()
        {
            const string localhost = "http://localhost";

            var expected = $"{localhost}/projects/1/files.xml";

            Assert.Throws<RedmineException>(() => UrlBuilder
                .Create(localhost, MimeType.Xml)
                .CreateUrl<File>(null)
                .Build());
        }
    }
}