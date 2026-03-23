using System.Net;
using System.Net.Http;
using System.Text;
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

        public async Task<GetMasterDataResult> GetMasterData(int areaId)
        {
            var payload = new GetMasterDataRequestDto
            {
                areaId = areaId
            };
            using var request = CreateRequest(payload);
            using var response = await _service.SendAsync(
                request,
                CancellationToken.None,
                _model.APIConfig.TimeoutMS);

            _log.Write("response.StatusCode: " + response.StatusCode);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                var responseDto = JsonUtility.FromJson<GetMasterDataResponseDto>(json);
                return new GetMasterDataResult
                {
                    isSuccess = true,
                    statusCode = (long)response.StatusCode,
                    areaId = responseDto.areaId,
                    ms_categories = responseDto.ms_categories,
                    ms_areas = responseDto.ms_areas,
                    ms_inspectors = responseDto.ms_inspectors,
                    ms_detection_types = responseDto.ms_detection_types,
                    ms_judge_conditions = responseDto.ms_judge_conditions,
                    ms_inspection_targets = responseDto.ms_inspection_targets,
                    ms_inspection_items = responseDto.ms_inspection_items,
                    ms_inspection_target_attachments = responseDto.ms_inspection_target_attachments,
                };
            }
            else
            {
                return null;
            }
        }

        HttpRequestMessage CreateRequest(GetMasterDataRequestDto payload)
        {
            return new HttpRequestMessage(HttpMethod.Post, _model.GetMasterDataEndpoint)
            {
                Content = new StringContent(JsonUtility.ToJson(payload), Encoding.UTF8, "application/json")
            };
        }
    }
}
