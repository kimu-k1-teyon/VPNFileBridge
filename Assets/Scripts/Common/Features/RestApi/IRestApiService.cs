using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiService
    {
        Task<ApiHttpResponse> PostMultipartAsync(
            string url,
            string uploadId,
            byte[] fileBytes,
            string fileName,
            CancellationToken cancellationToken);

        Task<HttpResponseMessage> PostAsync(string json, string url);
        Task<string> GetAsync(string url);
    }

}
