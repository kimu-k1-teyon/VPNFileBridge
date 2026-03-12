using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiMasterService
    {
        Task<GetMasterDataResult> GetAsync(int targets, int sosaKashoId, CancellationToken cancellationToken);
    }
}
