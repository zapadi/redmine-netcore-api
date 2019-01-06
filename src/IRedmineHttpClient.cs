using System;
using System.Threading;
using System.Threading.Tasks;
using RedmineApi.Core.Types;

namespace RedmineApi.Core
{
    internal interface IRedmineHttpClient : IDisposable
    {
        Task<T> GetAsync<T>(Uri uri, MimeType mimeType, CancellationToken cancellationToken) where T : class, new();

        Task<PaginatedResult<T>> ListAsync<T>(Uri uri, MimeType mimeType, CancellationToken cancellationToken) where T : class, new();

        Task<T> PutAsync<T>(Uri uri, T data, MimeType mimeType, CancellationToken cancellationToken) where T : class, new();

        Task<T> PostAsync<T>(Uri uri, T data, MimeType mimeType, CancellationToken cancellationToken) where T : class, new();

        Task<byte[]> DownloadFileAsync(Uri uri, MimeType mimeType, CancellationToken cancellationToken);

        Task<Upload> UploadFileAsync(Uri uri, byte[] bytes, MimeType mimeType, CancellationToken cancellationToken);

        Task<Upload> UploadAttachmentAsync(Uri uri, string attachmentContent, MimeType mimeType, CancellationToken cancellationToken);

        Task<int> CountAsync<T>(Uri uri, MimeType mimeType, CancellationToken cancellationToken) where T : new();
    }
}