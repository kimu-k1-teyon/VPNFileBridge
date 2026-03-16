using System;
using System.Net;
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
                var response = await _service.PostAsync(JsonUtility.ToJson(payload), _model.DownloadEndpoint);
                _log.Write("response.StatusCode: " + response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _log.Write("FileName: " + response.Content.Headers.ContentDisposition.FileName);
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
            finally
            {

            }

        }
    }
}
