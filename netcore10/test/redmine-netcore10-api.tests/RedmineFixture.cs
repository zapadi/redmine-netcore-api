using System.Diagnostics;
using ClassLibrary;

namespace Tests
{
    public class RedmineFixture
    {
        public RedmineManager RedmineManager { get; set; }

        public RedmineFixture()
        {
            SetMimeTypeXML();
          //  SetMimeTypeJSON();
        }

        //[Conditional("JSON")]
        //private void SetMimeTypeJSON()
        //{
        //    RedmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.Json);
        //}

       // [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            RedmineManager = new RedmineManager("http://padi.m.redmine.org/", "901e175d910799a8c9bb025d1d3c5bcb1a2a8ddc");
        }
    }
}