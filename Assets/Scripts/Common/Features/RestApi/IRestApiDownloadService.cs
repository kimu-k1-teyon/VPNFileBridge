using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiDownloadService
    {
        Task<byte[]> DownloadAsync(string uploadId);
    }
}
