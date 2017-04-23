using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
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

namespace Redmine.Net.Api
{
    public class RedmineManager : IDisposable
    {
        public const int DEFAULT_PAGE_SIZE_VALUE = 25;

        public RedmineManager(string host, string apiKey, MimeType mimeType = MimeType.Xml,
            IRedmineHttpSettings httpClientHandler = null)
        {
            host.EnsureValidHost();
            Host = host;
            MimeType = mimeType;

            var clientHandler = httpClientHandler != null
                ? httpClientHandler.Build()
                : DefaultRedmineHttpSettings.Builder().Build();
            RedmineHttp = new RedmineHttpClient(clientHandler);

            ApiKey = apiKey;
        }

        public RedmineManager(string host, IAuthentication authentication, MimeType mimeType = MimeType.Xml,
            IRedmineHttpSettings httpClientHandler = null)
        {
            host.EnsureValidHost();
            Host = host;
            MimeType = mimeType;

            var clientHandler = httpClientHandler != null
                ? httpClientHandler.SetAuthentication(authentication?.Build()).Build()
                : DefaultRedmineHttpSettings.Builder().SetAuthentication(authentication?.Build()).Build();
            RedmineHttp = new RedmineHttpClient(clientHandler);
        }

        public MimeType MimeType { get; }

        public string Host { get; }

        public int PageSize { get; set; }

        public string ImpersonateUser
        {
            get => RedmineHttp.ImpersonateUser;
            set => RedmineHttp.ImpersonateUser = value;
        }

        public string ApiKey
        {
            get => RedmineHttp.ApiKey;
            set => RedmineHttp.ApiKey = value;
        }

        private RedmineHttpClient RedmineHttp { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RedmineManager()
        {
            Dispose(false);
        }

        public async Task<TData> Create<TData>(TData data)
            where TData : class, new()
        {
            return await Create(null, data);
        }

        public async Task<TData> Create<TData>(string ownerId, TData data)
            where TData : class, new()
        {
            var uri = UrlBuilder.Create(Host, MimeType).CreateUrl<TData>(ownerId).Build();
            var response = await RedmineHttp.Post(new Uri(uri), data, MimeType);
            return response;
        }

        public async Task<TData> Get<TData>(string id, NameValueCollection parameters)
            where TData : class, new()
        {
            var uri = UrlBuilder.Create(Host, MimeType).GetUrl<TData>(id).SetParameters(parameters).Build();
            var response = await RedmineHttp.Get<TData>(new Uri(uri), MimeType);
            return response;
        }

        public async Task<List<TData>> ListAll<TData>(NameValueCollection parameters)
            where TData : class, new()
        {
            int totalCount = 0, pageSize = 0, offset = 0;
            var isLimitSet = false;
            List<TData> resultList = null;

            if (parameters == null)
            {
                parameters = new NameValueCollection();
            }
            else
            {
                isLimitSet = int.TryParse(parameters[RedmineKeys.LIMIT], out pageSize);
                int.TryParse(parameters[RedmineKeys.OFFSET], out offset);
            }
            if (pageSize == default(int))
            {
                pageSize = PageSize > 0 ? PageSize : DEFAULT_PAGE_SIZE_VALUE;
                parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            }

            do
            {
                parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));
                var tempResult = await List<TData>(parameters);
                if (tempResult != null)
                    if (resultList == null)
                    {
                        resultList = tempResult.Items;
                        totalCount = isLimitSet ? pageSize : tempResult.Total;
                    }
                    else
                    {
                        resultList.AddRange(tempResult.Items);
                    }
                offset += pageSize;
            } while (offset < totalCount);

            return resultList;
        }

        public async Task<PaginatedResult<TData>> List<TData>(NameValueCollection parameters)
            where TData : class, new()
        {
            var uri = UrlBuilder.Create(Host, MimeType)
                .ItemsUrl<TData>(parameters)
                .SetParameters(parameters)
                .Build();

            var response = await RedmineHttp.List<TData>(new Uri(uri), MimeType).ConfigureAwait(false);

            return response;
        }

        public async Task<TData> Update<TData>(string id, TData data) where TData : class, new()
        {
            return await Update(id, data, null);
        }

        public async Task<TData> Update<TData>(string id, TData data, string projectId) where TData : class, new()
        {
            var uri = UrlBuilder.Create(Host, MimeType).UploadUrl(id, data, projectId).Build();
            var response = await RedmineHttp.Put(new Uri(uri), data, MimeType).ConfigureAwait(false);
            return response;
        }

        public async Task<HttpStatusCode> Delete<T>(string id) where T : class, new()
        {
            return await Delete<T>(id, null);
        }

        public async Task<HttpStatusCode> Delete<T>(string id, string reasignedId) where T : class, new()
        {
            var uri = UrlBuilder.Create(Host, MimeType).DeleteUrl<T>(id).Build();
            var response = await RedmineHttp.Delete(new Uri(uri), MimeType).ConfigureAwait(false);
            return response;
        }

        private void ReleaseUnmanagedResources()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
                RedmineHttp?.Dispose();
        }
    }
}