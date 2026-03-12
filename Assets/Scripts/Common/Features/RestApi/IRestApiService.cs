using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiService
    {
        Task<ApiHttpResponse> GetAsync(string url, CancellationToken cancellationToken);
        Task<ApiHttpResponse> PostJsonAsync(string url, string jsonBody, CancellationToken cancellationToken);
        Task<ApiHttpResponse> PostMultipartAsync(
            string url,
            byte[] fileBytes,
            string fileName,
            string metaJson,
            CancellationToken cancellationToken);
    }
}
