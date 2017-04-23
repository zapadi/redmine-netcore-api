using System;
using System.Net.Http.Headers;

namespace Redmine.Net.Api
{
    public class OAuthAuthentication : IAuthentication
    {
        private const string AUTHENTICATION_BEARER = "Bearer";

        private OAuthAuthentication(string token)
        {
            Token = token;
        }

        public string Token { get; }

        public string Type => AUTHENTICATION_BEARER;

        public AuthenticationHeaderValue Build()
        {
            return new AuthenticationHeaderValue(AUTHENTICATION_BEARER, Token);
        }

        public static IAuthentication Create(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            return new OAuthAuthentication(token);
        }
    }
}