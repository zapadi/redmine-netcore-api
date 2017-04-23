using System;
using System.Net.Http.Headers;
using System.Text;

namespace Redmine.Net.Api
{
    public class BasicAuthentication : IAuthentication
    {
        private const string AUTHENTICATION_BASIC = "Basic";

        private BasicAuthentication(string username, string password)
        {
            UserName = username;
            Password = password;
        }

        public string UserName { get; }
        public string Password { get; }

        public string Type => AUTHENTICATION_BASIC;

        public AuthenticationHeaderValue Build()
        {
            var value = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{Password}"));
            return new AuthenticationHeaderValue(AUTHENTICATION_BASIC, value);
        }

        public static BasicAuthentication Create(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            return new BasicAuthentication(username, password);
        }
    }
}