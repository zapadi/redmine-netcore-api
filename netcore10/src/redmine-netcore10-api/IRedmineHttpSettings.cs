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

        IRedmineHttpSettings SetWebProxy(IWebProxy webProxy);

        IRedmineHttpSettings SetAuthentication(AuthenticationHeaderValue authenticationHeaderValue);

        IRedmineHttpSettings SetProxyAuthentication(AuthenticationHeaderValue proxyAuthenticationHeaderValue);

        IRedmineHttpSettings SetCookieContainer(CookieContainer cookieContainer);

        IRedmineHttpSettings SetDefaultCredentials(ICredentials defaultCredentials);

        IRedmineHttpSettings SetProxyCredentials(ICredentials proxyCredentials);

        IRedmineHttpSettings SetSslProtocols(SslProtocols sslProtocols);

        IRedmineHttpSettings SetDecompressionMethods(DecompressionMethods decompressionMethods);

        IRedmineHttpSettings SetUseProxy(bool useProxy);

        IRedmineHttpSettings SetUseDefaultCredentials(bool useDefaultCredentials);

        IRedmineHttpSettings SetUseCookies(bool useCookies);

        IRedmineHttpSettings SetPreAuthenticate(bool preAuthenticate);


        HttpClientHandler Build();
    }
}