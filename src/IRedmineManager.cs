/*
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RedmineApi.Core.Types;

namespace RedmineApi.Core
{
    public interface IRedmineManager : IDisposable
    {
        string ApiKey { get; }
        string ImpersonateUser { get; set; }
        MimeType MimeType { get; }

         string Host { get; }

         int PageSize { get; set; }

         Task<TData> Create<TData>(TData data, CancellationToken cancellationToken) where TData : class, new();

         Task<TData> Create<TData>(string ownerId, TData data, CancellationToken cancellationToken)
             where TData : class, new();

         Task<TData> Get<TData>(string id, NameValueCollection parameters, CancellationToken cancellationToken)
             where TData : class, new();

         Task<List<TData>> ListAll<TData>(NameValueCollection parameters, CancellationToken cancellationToken)
             where TData : class, new();

         Task<PaginatedResult<TData>> List<TData>(NameValueCollection parameters, CancellationToken cancellationToken)
             where TData : class, new();

         Task<TData> Update<TData>(string id, TData data, CancellationToken cancellationToken) where TData : class, new ();

         Task<TData> Update<TData>(string id, TData data, string projectId, CancellationToken cancellationToken) where TData : class, new();

         Task<HttpStatusCode> Delete<T>(string id, CancellationToken cancellationToken) where T : class, new();

         Task<HttpStatusCode> Delete<T>(string id, string reassignedId, CancellationToken cancellationToken) where T : class, new();

         Task<Upload> UploadFile(byte[] fileBytes, CancellationToken cancellationToken);

         Task<Upload> UploadAttachment(int issueId, Attachment attachment, CancellationToken cancellationToken);

         Task<byte[]> DownloadFile(string address, CancellationToken cancellationToken);

         Task<int> CountAsync<T>(Uri uri, CancellationToken cancellationToken) where T : new();
    }
}