using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiDownloadService
    {
        Task<DownloadResult> DownloadAsync(string uploadId, CancellationToken cancellationToken);
    }
}
