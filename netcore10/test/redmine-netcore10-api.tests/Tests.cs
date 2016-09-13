using System;
using Xunit;
using ClassLibrary;
using Redmine.Net.Api.Types;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public  void Test1() 
        {
            RedmineManager rm = new RedmineManager("<your-host>","<your-api-key>", MimeType.Xml);
            var project =rm.Get<Project>("<project-id>", null).Result;
            Assert.NotNull(project);
            Assert.True(project.Id == 92, $"Found project with id:{project.Id} instead of 92");
        }
    }
}
