using System.Collections.Specialized;

namespace Redmine.Net.Api.Internals
{
    public interface IUrlBuild
    {
        IUrlBuild SetParameters(NameValueCollection parameters);
        string Build();
    }
}