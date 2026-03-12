using System;
using System.Threading;
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
        // TODO: Component 実装
        public async Task<GetMasterDataResult> GetAsync(int targets, int sosaKashoId, CancellationToken cancellationToken)
        {
            var payload = new GetMasterDataRequestDto
            {
                targets = targets,
                sosaKashoId = sosaKashoId
            };

            var response = await _service.PostJsonAsync(_model.GetMasterDataEndpoint, JsonUtility.ToJson(payload), cancellationToken);

            if (!response.IsTransportSuccess)
            {
                return GetMasterDataResult.Failure(response.StatusCode, response.ErrorMessage);
            }

            if (!response.IsSuccessStatusCode)
            {
                return GetMasterDataResult.Failure(response.StatusCode, "IsSuccessStatusCode: False");
            }

            if (string.IsNullOrWhiteSpace(response.ResponseText))
            {
                return GetMasterDataResult.Failure(response.StatusCode, "Empty response body.");
            }

            var dto = JsonUtility.FromJson<GetMasterDataResponseDto>(response.ResponseText);
            if (dto == null)
            {
                return GetMasterDataResult.Failure(response.StatusCode, "Invalid GetMasterData response.");
            }

            return GetMasterDataResult.Success(
                response.StatusCode,
                dto.sosaKashoId,
                dto.targets,
                dto.sampleA ?? Array.Empty<MasterDataItem>(),
                dto.sampleB ?? Array.Empty<MasterDataItem>());

        }
    }
}
