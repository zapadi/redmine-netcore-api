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