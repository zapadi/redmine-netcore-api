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

        Task<Upload> UploadAttachmentAsync(Uri uri, string attachmentContent,  CancellationToken cancellationToken);

        Task<int> CountAsync<T>(Uri uri,  CancellationToken cancellationToken) where T : new();
    }
}