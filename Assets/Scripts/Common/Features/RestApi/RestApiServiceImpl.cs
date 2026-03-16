using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Scripts.Common.Features.Config;
using Scripts.Common.Log;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiServiceImpl : IRestApiService
    {
        [Inject] RestApiModel _model;
        [Inject] IConfigEnviourment _configEnviourment;
        [Inject] ILogService _log;


        public async Task<HttpResponseMessage> PostAsync(string json, string url)
        {
            StringContent content = null;
            try
            {
                _log.Write("StartPostAsync");
                // リクエストボディの設定
                content = new StringContent(json, Encoding.UTF8, "application/json");
                // 通信処理実行
                var postTask = _model.Client.PostAsync(url, content);

                // ソフトタイムアウト
                var timeoutTask = Task.Delay(_model.APIConfig.TimeoutMS);
                var completed = await Task.WhenAny(postTask, timeoutTask);

                if (completed != postTask)
                {
                    _ = postTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                    _log.Write("POST通信タイムアウトエラー");
                    return null;
                }
                return await postTask;
            }
            catch (Exception e)
            {
                _log.Write($"POST通信エラー {e}");
                return null;
            }
            finally
            {
                _log.Write("Finish Task");
            }
        }

        public async Task<string> GetAsync(string url)
        {
            try
            {
                // 通信処理実行
                HttpResponseMessage httpResponse = await _model.Client.GetAsync(url);

                // ステータスコードが200以外の場合、業務エラー
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    _log.Write("通信エラー　コード：" + (int)httpResponse.StatusCode);
                    return null;
                }

                // 結果を返却
                return await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                _log.Write($"GET通信エラー {e}");
                return null;
            }
        }

        public async Task<ApiHttpResponse> PostMultipartAsync(
            string url,
            string uploadId,
            byte[] fileBytes,
            string fileName,
            CancellationToken cancellationToken)
        {
            var formSections = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("uploadId", uploadId, "text/plain"),
                new MultipartFormFileSection("file", fileBytes, fileName, "application/octet-stream")
            };

            using (var request = UnityWebRequest.Post(url, formSections))
            {
                request.SetRequestHeader("Accept", "application/json");
                return await SendAsync(request, cancellationToken);
            }
        }

        async Task<ApiHttpResponse> SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            await Send(request, cancellationToken);

            var responseBytes = request.downloadHandler != null
                ? request.downloadHandler.data ?? Array.Empty<byte>()
                : Array.Empty<byte>();

            var responseText = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
            var isTransportSuccess =
                request.result != UnityWebRequest.Result.ConnectionError &&
                request.result != UnityWebRequest.Result.DataProcessingError;
            var errorMessage = !string.IsNullOrWhiteSpace(request.error)
                ? request.error
                : string.Empty;

            var headers = request.GetResponseHeaders() ?? new Dictionary<string, string>();

            return new ApiHttpResponse(
                isTransportSuccess,
                request.responseCode,
                responseText,
                errorMessage,
                responseBytes,
                headers);
        }

        Task<UnityWebRequest> Send(UnityWebRequest request, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            var registration = cancellationToken.Register(() =>
            {
                if (!request.isDone)
                {
                    request.Abort();
                }
            });

            var operation = request.SendWebRequest();
            operation.completed += _ =>
            {
                registration.Dispose();
                tcs.TrySetResult(request);
            };

            return tcs.Task;
        }
    }


}
