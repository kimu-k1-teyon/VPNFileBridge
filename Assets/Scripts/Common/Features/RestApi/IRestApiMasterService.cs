using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiMasterService
    {
        Task<GetMasterDataResult> GetMasterData(int areaId);
    }
}
