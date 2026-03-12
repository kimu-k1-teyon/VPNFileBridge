using System;
using System.Collections.Generic;
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

        public async Task<ApiHttpResponse> GetAsync(string url, CancellationToken cancellationToken)
        {
            using (var request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Accept", "application/json");
                return await SendAsync(request, cancellationToken);
            }
        }

        public async Task<ApiHttpResponse> PostJsonAsync(string url, string jsonBody, CancellationToken cancellationToken)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(jsonBody ?? string.Empty);
            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
                request.SetRequestHeader("Accept", "application/json");
                return await SendAsync(request, cancellationToken);
            }
        }

        public async Task<ApiHttpResponse> PostMultipartAsync(
            string url,
            byte[] fileBytes,
            string fileName,
            string metaJson,
            CancellationToken cancellationToken)
        {
            var formSections = new List<IMultipartFormSection>
            {
                new MultipartFormFileSection("file", fileBytes, fileName, "application/octet-stream")
            };

            if (!string.IsNullOrWhiteSpace(metaJson))
            {
                formSections.Add(new MultipartFormDataSection("meta", metaJson, Encoding.UTF8.ToString()));
            }

            using (var request = UnityWebRequest.Post(url, formSections))
            {
                request.SetRequestHeader("Accept", "application/json");
                return await SendAsync(request, cancellationToken);
            }
        }

        private static async Task<ApiHttpResponse> SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            await UnityWebRequestAsync.SendAsync(request, cancellationToken);

            var responseText = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
            var isTransportSuccess =
                request.result != UnityWebRequest.Result.ConnectionError &&
                request.result != UnityWebRequest.Result.DataProcessingError;
            var errorMessage = !string.IsNullOrWhiteSpace(request.error)
                ? request.error
                : string.Empty;

            return new ApiHttpResponse(isTransportSuccess, request.responseCode, responseText, errorMessage);
        }























        [Serializable]
        public class UploadRequest
        {
            public string requestedPath;
            public string fileBase64;
        }

        [Serializable]
        public class DownloadRequest
        {
            public string requestedPath;
        }

        // Backward-compatible upload entrypoint.
        public async Task<string> PostAsync()
        {
            return await PostUploadAsync(
                requestedPath: "documents/from-unity.json",
                fileBase64: "eyJoZWxsbyI6InVuaXR5In0="
            );
        }

        public async Task<string> PostUploadAsync(string requestedPath, string fileBase64)
        {
            var req = new UploadRequest
            {
                requestedPath = requestedPath,
                fileBase64 = fileBase64,
            };
            var json = JsonUtility.ToJson(req);
            return await PostAsync("/api/UploadFile", json);
        }

        public async Task<string> PostDownloadAsync(string requestedPath)
        {
            var req = new DownloadRequest
            {
                requestedPath = requestedPath,
            };
            var json = JsonUtility.ToJson(req);
            return await PostAsync("/api/DownloadFile", json);
        }

        private async Task<string> PostAsync(string url, string json)
        {
            try
            {
                _log.Write("StartTask");
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postTask = _model.Client.PostAsync(url, content);
                var timeoutTask = Task.Delay(_model.Client.Timeout);

                _log.Write("_model.Client.Timeout: " + _model.Client.Timeout.ToString());
                _log.Write("StartPost: " + url);
                var completed = await Task.WhenAny(postTask, timeoutTask);

                _log.Write("FinishPost: " + url);

                if (completed != postTask)
                {
                    _ = postTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                    _log.Write("POST通信タイムアウトエラー");
                    return null;
                }

                using var httpResponse = await postTask;
                _log.Write("httpResponse.StatusCode: " + httpResponse.StatusCode);

                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    _log.Write("通信エラー　コード：" + (int)httpResponse.StatusCode);
                    var errorResponse = await httpResponse.Content.ReadAsStringAsync();
                    _log.Write("通信エラー　レスポンス：" + errorResponse);
                    return null;
                }

                var response = await httpResponse.Content.ReadAsStringAsync();
                _log.Write("POST通信成功");
                return response;
            }
            catch (Exception e)
            {
                _log.Write("通信失敗: " + e.ToString());
                _log.Write("通信失敗: " + e.Message);
                return null;
            }
            finally
            {
                _log.Write("POST通信終了");
            }
        }
    }


    public class ApiHttpResponse
    {
        public ApiHttpResponse(bool isTransportSuccess, long statusCode, string responseText, string errorMessage)
        {
            IsTransportSuccess = isTransportSuccess;
            StatusCode = statusCode;
            ResponseText = responseText ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public bool IsTransportSuccess { get; }
        public long StatusCode { get; }
        public string ResponseText { get; }
        public string ErrorMessage { get; }

        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;
    }


    public static class UnityWebRequestAsync
    {
        public static Task<UnityWebRequest> SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
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
