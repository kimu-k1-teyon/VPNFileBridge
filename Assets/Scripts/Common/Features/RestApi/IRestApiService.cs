using System.Threading.Tasks;

namespace Scripts.Common.Features.RestApi
{
    public interface IRestApiService
    {
        Task<string> PostAsync(string json, string url);
        Task<string> PostAsync();
        Task<string> PostAsync2();
    }
}
