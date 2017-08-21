/*
   Copyright 2016 - 2017 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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
        bool AllowAutoRedirect { get; }
        bool UseProxy { get;  }
        bool UseDefaultCredentials { get; }
        bool UseCookies { get;  }
        bool PreAuthenticate { get;  }
        int MaxAutomaticRedirections { get; }

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

        IRedmineHttpSettings SetAllowAutoRedirect(bool allowAutoRedirect);

        IRedmineHttpSettings SetMaxAutomaticRedirections(int maxAutomaticRedirections);

        HttpClientHandler Build();
    }
}