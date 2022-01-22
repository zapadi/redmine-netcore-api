using System.Collections.Specialized;
using RedmineApi.Core.Exceptions;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Types;
using Xunit;

namespace RedmineApi.Core.UnitTests
{
    public sealed class UrlBuilderUnitTests
    {
        [Fact]
        public void ShouldReturnCreateFileUrl()
        {
            const string LOCALHOST = "http://localhost";

            var expected = $"{LOCALHOST}/projects/1/files.xml";

            // var actual = UrlBuilder
            //     .Create(LOCALHOST, MimeType.Xml)
            //     .CreateUrl<File>("1")
            //     .Build();
            //
            // Assert.True(expected == actual);
        }

        [Fact]
        public void ShouldReturnGetFileUrl()
        {
            const string LOCALHOST = "http://localhost";

            var expected = $"{LOCALHOST}/projects/1/files.xml";

            // var actual = UrlBuilder
            //     .Create(LOCALHOST, MimeType.Xml)
            //     .ItemsUrl<File>(new NameValueCollection {{RedmineKeys.PROJECT_ID, "1"}})
            //     .Build();
            //
            // Assert.True(expected == actual);
        }

        [Fact]
        public void ThrowExceptionWhenCreateFileUrlWithout_Project()
        {
            const string LOCALHOST = "http://localhost";

            // Assert.Throws<RedmineException>(() => UrlBuilder
            //     .Create(LOCALHOST, MimeType.Xml)
            //     .CreateUrl<File>(null)
            //     .Build());
        }
    }
}
