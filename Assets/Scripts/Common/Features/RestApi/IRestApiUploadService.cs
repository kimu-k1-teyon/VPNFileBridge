using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiUploadService
    {
        Task<UploadResult> UploadAsync(string filePath, CancellationToken cancellationToken);
    }
}
