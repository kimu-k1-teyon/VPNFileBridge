using System;
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
    public class RestApiDownloadServiceImpl : IRestApiDownloadService
    {
        [Inject] IRestApiService _service;
        [Inject] RestApiModel _model;
        [Inject] ILogService _log;

        public async Task<byte[]> DownloadAsync(string uploadId)
        {
            var payload = new DownloadRequestDto
            {
                uploadId = uploadId
            };

            try
            {
                using var request = CreateRequest(payload);
                using var response = await _service.SendAsync(
                    request,
                    CancellationToken.None,
                    _model.APIConfig.TimeoutMS);
                _log.Write("response.StatusCode: " + response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _log.Write("FileName: " + response.Content.Headers.ContentDisposition?.FileName);
                    //拡張子別に分けて返せば良さそう。
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                _log.Write(e.ToString());
                return null;
            }
        }

        HttpRequestMessage CreateRequest(DownloadRequestDto payload)
        {
            return new HttpRequestMessage(HttpMethod.Post, _model.DownloadEndpoint)
            {
                Content = new StringContent(JsonUtility.ToJson(payload), Encoding.UTF8, "application/json")
            };
        }
    }
}
