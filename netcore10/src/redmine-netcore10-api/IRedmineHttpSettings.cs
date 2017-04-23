using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace Redmine.Net.Api
{
    public interface IRedmineHttpSettings
    {
        IWebProxy WebProxy { get;  }
        AuthenticationHeaderValue Authentication { get;  }
        AuthenticationHeaderValue ProxyAuthentication { get;  }
        CookieContainer CookieContainer { get;  }
        ICredentials DefaultCredentials { get;  }
        ICredentials ProxyCredentials { get; }
        SslProtocols SslProtocols { get;  }
        bool UseProxy { get;  }
        bool UseDefaultCredentials { get; }
        bool UseCookies { get;  }
        bool PreAuthenticate { get;  }
        DecompressionMethods DecompressionMethods { get;  }

        HttpClientHandler Build();
    }
}