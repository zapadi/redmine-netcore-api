using System.Net.Http.Headers;

namespace Redmine.Net.Api
{
    public interface IAuthentication
    {
        string Type { get; }
        AuthenticationHeaderValue Build();
    }
}