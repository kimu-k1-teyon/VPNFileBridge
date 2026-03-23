using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiUploadService
    {
        Task<HttpResponseMessage> UploadAsync(string uploadId, string filePath, CancellationToken cancellationToken);
    }
}
