using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace Redmine.Net.Api
{
    internal class DefaultRedmineHttpSettings : IRedmineHttpSettings
    {
        private DefaultRedmineHttpSettings()
        {
            DecompressionMethods = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;
        }

        public static DefaultRedmineHttpSettings Builder()
        {
            return new DefaultRedmineHttpSettings();
        }

        public IWebProxy WebProxy { get; private set; }
        public AuthenticationHeaderValue Authentication { get; private set; }
        public AuthenticationHeaderValue ProxyAuthentication { get; private set; }
        public CookieContainer CookieContainer { get; private set; }
        public ICredentials DefaultCredentials { get; private set; }
        public ICredentials ProxyCredentials { get; private set; }
        public SslProtocols SslProtocols { get; private set; }
        public DecompressionMethods DecompressionMethods { get; private set; }
        public bool UseProxy { get; private set; }
        public bool UseDefaultCredentials { get; private set; }
        public bool UseCookies { get; private set; }
        public bool PreAuthenticate { get; private set; }
        
        public IRedmineHttpSettings SetWebProxy(IWebProxy webProxy)
        {
            WebProxy = webProxy;
            return this;
        }

        public IRedmineHttpSettings SetAuthentication(AuthenticationHeaderValue authenticationHeaderValue)
        {
            Authentication = authenticationHeaderValue;
            return this;
        }

        public IRedmineHttpSettings SetProxyAuthentication(AuthenticationHeaderValue proxyAuthenticationHeaderValue)
        {
            ProxyAuthentication = proxyAuthenticationHeaderValue;
            return this;
        }

        public IRedmineHttpSettings SetCookieContainer(CookieContainer cookieContainer)
        {
            CookieContainer = cookieContainer;
            return this;
        }

        public IRedmineHttpSettings SetDefaultCredentials(ICredentials defaultCredentials)
        {
            DefaultCredentials = defaultCredentials;
            return this;
        }

        public IRedmineHttpSettings SetProxyCredentials(ICredentials proxyCredentials)
        {
            ProxyCredentials = proxyCredentials;
            return this;
        }

        public IRedmineHttpSettings SetSslProtocols(SslProtocols sslProtocols)
        {
            SslProtocols = sslProtocols;
            return this;
        }

        public IRedmineHttpSettings SetDecompressionMethods(DecompressionMethods decompressionMethods)
        {
            DecompressionMethods = decompressionMethods;
            return this;
        }

        public IRedmineHttpSettings SetUseProxy(bool useProxy)
        {
            UseProxy = useProxy;
            return this;
        }

        public IRedmineHttpSettings SetUseDefaultCredentials(bool useDefaultCredentials)
        {
            UseDefaultCredentials = useDefaultCredentials;
            return this;
        }

        public IRedmineHttpSettings SetUseCookies(bool useCookies)
        {
            UseCookies = useCookies;
            return this;
        }

        public IRedmineHttpSettings SetPreAuthenticate(bool preAuthenticate)
        {
            PreAuthenticate = preAuthenticate;
            return this;
        }


        public HttpClientHandler Build()
        {
            HttpClientHandler handler = new HttpClientHandler();

            handler.AutomaticDecompression = DecompressionMethods;
            handler.CookieContainer = CookieContainer;

            if (UseCookies && handler.CookieContainer == null)
                handler.CookieContainer = new CookieContainer();

            handler.Proxy = WebProxy;
            handler.UseProxy = UseProxy;

            if (UseProxy && WebProxy != null)
            {
                handler.Proxy.Credentials = ProxyCredentials;
            }

            handler.Credentials = DefaultCredentials;
            handler.PreAuthenticate = PreAuthenticate;

            handler.SslProtocols = SslProtocols;
            handler.UseCookies = UseCookies;
            handler.UseDefaultCredentials = UseDefaultCredentials;

            return handler;
        }
    }
}