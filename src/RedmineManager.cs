﻿/*
   Copyright 2016 - 2019 Adrian Popescu.

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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RedmineApi.Core.Authentication;
using RedmineApi.Core.Internals;
using RedmineApi.Core.Types;

[assembly: InternalsVisibleTo("Zapadi.Redmine.Api.UnitTests")]

namespace RedmineApi.Core
{
    public class RedmineManager : IRedmineManager
    {
        private const int DEFAULT_PAGE_SIZE_VALUE = 25;

        public RedmineManager(string host, string apiKey, MimeType mimeType = MimeType.Xml,
            IRedmineHttpSettings httpClientHandler = null)
        {
            EnsureValidHost(host);
            Host = host;
            MimeType = mimeType;

            var clientHandler = httpClientHandler != null
                ? httpClientHandler.Build()
                : DefaultRedmineHttpSettings.Create().Build();
            redmineHttp = new RedmineHttpClient(clientHandler, mimeType);

            ApiKey = apiKey;
        }

        public RedmineManager(string host, IAuthentication authentication, MimeType mimeType = MimeType.Xml,
            IRedmineHttpSettings httpClientHandler = null)
        {
            EnsureValidHost(host);

            if (authentication == null)
            {
                throw new ArgumentNullException(nameof(authentication));
            }

            Host = host;
            MimeType = mimeType;

            var auth = authentication.Build();

            var clientHandler = httpClientHandler != null
                ? httpClientHandler.SetAuthentication(auth).Build()
                : DefaultRedmineHttpSettings.Create().SetAuthentication(auth).Build();
            redmineHttp = new RedmineHttpClient(clientHandler, mimeType);
        }

        public MimeType MimeType { get; }

        public string Host { get; }

        public int PageSize { get; set; }

        /// <summary>
        ///     Gets or sets the impersonate user.
        ///     This only works when using the API with an administrator account, this header will be ignored when using the API
        ///     with a regular user account
        ///     If the login specified with the X-Redmine-Switch-User header does not exist or is not active, you will receive a
        ///     412 error response.
        /// </summary>
        /// <value>The impersonate user.</value>
        public string ImpersonateUser
        {
            get => redmineHttp.ImpersonateUser;
            set => redmineHttp.ImpersonateUser = value;
        }

        public string ApiKey
        {
            get => redmineHttp.ApiKey;
            private set => redmineHttp.ApiKey = value;
        }

        private readonly RedmineHttpClient redmineHttp;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RedmineManager()
        {
            Dispose(false);
        }

        public async Task<TData> Create<TData>(TData data, CancellationToken cancellationToken)
            where TData : class, new()
        {
            return await Create(null, data, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TData> Create<TData>(string ownerId, TData data, CancellationToken cancellationToken)
            where TData : class, new()
        {
            var uri = UrlBuilder
               .Create(Host, MimeType)
               .CreateUrl<TData>(ownerId)
               .Build();

            var response = await redmineHttp.PostAsync(new Uri(uri), data, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task<TData> Get<TData>(string id, NameValueCollection parameters, CancellationToken cancellationToken)
            where TData : class, new()
        {
            var uri = UrlBuilder
               .Create(Host, MimeType)
               .GetUrl<TData>(id)
               .SetParameters(parameters)
               .Build();

            var response = await redmineHttp.GetAsync<TData>(new Uri(uri), cancellationToken).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        ///     Lists all.
        /// </summary>
        /// <returns>The all.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TData">The 1st type parameter.</typeparam>
        public async Task<List<TData>> ListAll<TData>(NameValueCollection parameters, CancellationToken cancellationToken)
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
                var tempResult = await List<TData>(parameters, cancellationToken).ConfigureAwait(false);
                if (tempResult != null)
                {
                    if (resultList == null)
                    {
                        resultList = tempResult.Items;
                        totalCount = isLimitSet ? pageSize : tempResult.Total;
                    }
                    else
                    {
                        resultList.AddRange(tempResult.Items);
                    }
                }

                offset += pageSize;
            }
            while (offset < totalCount);

            return resultList;
        }

        /// <summary>
        ///     List the specified parameters.
        /// </summary>
        /// <returns>
        ///     won't return all the objects available in your database. Redmine 1.1.0 introduces a common way to query such
        ///     resources using the following parameters:
        ///     offset: the offset of the first object to retrieve
        ///     limit: the number of items to be present in the response(default is 25, maximum is 100)
        /// </returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="cancellationToken"></param>
        public async Task<PaginatedResult<TData>> List<TData>(NameValueCollection parameters, CancellationToken cancellationToken)
            where TData : class, new()
        {
            var uri = UrlBuilder.Create(Host, MimeType)
               .ItemsUrl<TData>(parameters)
               .SetParameters(parameters)
               .Build();

            var response = await redmineHttp.ListAsync<TData>(new Uri(uri), cancellationToken).ConfigureAwait(false);

            return response;
        }

        public async Task<TData> Update<TData>(string id, TData data, CancellationToken cancellationToken) where TData : class, new()
        {
            return await Update(id, data, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TData> Update<TData>(string id, TData data, string projectId, CancellationToken cancellationToken) where TData : class, new()
        {
            var uri = UrlBuilder
               .Create(Host, MimeType)
               .UploadUrl(id, data, projectId)
               .Build();

            var response = await redmineHttp.PutAsync(new Uri(uri), data, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task<HttpStatusCode> Delete<T>(string id, CancellationToken cancellationToken) where T : class, new()
        {
            return await Delete<T>(id, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Delete<T>(string id, string reassignedId, CancellationToken cancellationToken) where T : class, new()
        {
            var uri = UrlBuilder
               .Create(Host, MimeType)
               .DeleteUrl<T>(id, reassignedId)
               .Build();

            var response = await redmineHttp.DeleteAsync(new Uri(uri), cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task<Upload> UploadFile(byte[] fileBytes, CancellationToken cancellationToken)
        {
            var uri = UrlBuilder.Create(Host, MimeType)
               .UploadFileUrl()
               .Build();

            var response = await redmineHttp.UploadFileAsync(new Uri(uri), fileBytes, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task<Upload> UploadAttachment(int issueId, Attachment attachment, CancellationToken cancellationToken)
        {
            var uri = UrlBuilder.Create(Host, MimeType)
               .AttachmentUpdateUrl(issueId)
               .Build();

            var response = await redmineHttp.UploadAttachmentAsync(new Uri(uri), attachment, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task<byte[]> DownloadFile(string address, CancellationToken cancellationToken)
        {
            var response = await redmineHttp.DownloadFileAsync(new Uri(address), cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task<int> CountAsync<T>(Uri uri, CancellationToken cancellationToken) where T : new()
        {
            var response = await redmineHttp.CountAsync<T>(uri, cancellationToken).ConfigureAwait(false);
            return response;
        }

        private void ReleaseUnmanagedResources()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                redmineHttp?.Dispose();
            }
        }

        private static void EnsureValidHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new UriFormatException("Host is not define!");
            }

            if (!Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute))
            {
                throw new UriFormatException($"Host '{host}' is not valid!");
            }
        }
    }
}