using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiService
    {
        Task<string> PostAsync();
        Task<string> PostDownloadAsync(string requestedPath);
        Task<ApiHttpResponse> GetAsync(string url, CancellationToken cancellationToken);
        Task<ApiHttpResponse> PostJsonAsync(string url, string jsonBody, CancellationToken cancellationToken);
    }
}
