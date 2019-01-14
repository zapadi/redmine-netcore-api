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
using System.Threading;
using System.Threading.Tasks;
using RedmineApi.Core.Types;

namespace RedmineApi.Core
{
    internal interface IRedmineHttpClient : IDisposable
    {
        Task<T> GetAsync<T>(Uri uri,  CancellationToken cancellationToken) where T : class, new();

        Task<PaginatedResult<T>> ListAsync<T>(Uri uri,  CancellationToken cancellationToken) where T : class, new();

        Task<T> PutAsync<T>(Uri uri, T data,  CancellationToken cancellationToken) where T : class, new();

        Task<T> PostAsync<T>(Uri uri, T data,  CancellationToken cancellationToken) where T : class, new();

        Task<byte[]> DownloadFileAsync(Uri uri,  CancellationToken cancellationToken);

        Task<Upload> UploadFileAsync(Uri uri, byte[] bytes,  CancellationToken cancellationToken);

        Task<Upload> UploadAttachmentAsync(Uri uri, Attachment attachment,  CancellationToken cancellationToken);

        Task<int> CountAsync<T>(Uri uri,  CancellationToken cancellationToken) where T : new();
    }
}