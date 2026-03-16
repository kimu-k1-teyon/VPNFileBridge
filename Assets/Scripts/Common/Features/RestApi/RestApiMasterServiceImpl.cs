using System.Net;
using System.Threading.Tasks;
using Scripts.Common.Log;
using UnityEngine;
using VContainer;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiMasterServiceImpl : IRestApiMasterService
    {
        [Inject] IRestApiService _service;
        [Inject] RestApiModel _model;
        [Inject] ILogService _log;

        public async Task<GetMasterDataResult> GetMasterData(int targets, int sosaKashoId)
        {
            var payload = new GetMasterDataRequestDto
            {
                targets = targets,
                sosaKashoId = sosaKashoId
            };
            var response = await _service.PostAsync(JsonUtility.ToJson(payload), _model.GetMasterDataEndpoint);

            _log.Write("response.StatusCode: " + response.StatusCode);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();

                return JsonUtility.FromJson<GetMasterDataResult>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
