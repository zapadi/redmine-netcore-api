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

using System;
using System.Net.Http.Headers;
using System.Text;

namespace RedmineApi.Core.Authentication
{
    public sealed class BasicAuthentication : IAuthentication
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
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            return new BasicAuthentication(username, password);
        }
    }
}