using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class ApiHttpResponse
    {
        readonly ReadOnlyDictionary<string, string> _headers;

        public ApiHttpResponse(
            bool isTransportSuccess,
            long statusCode,
            string responseText,
            string errorMessage,
            byte[] responseBytes,
            IDictionary<string, string> headers)
        {
            IsTransportSuccess = isTransportSuccess;
            StatusCode = statusCode;
            ResponseText = responseText ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
            ResponseBytes = responseBytes ?? Array.Empty<byte>();
            _headers = new ReadOnlyDictionary<string, string>(
                new Dictionary<string, string>(headers ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase));
        }

        public bool IsTransportSuccess { get; }
        public long StatusCode { get; }
        public string ResponseText { get; }
        public string ErrorMessage { get; }
        public byte[] ResponseBytes { get; }
        public IReadOnlyDictionary<string, string> Headers => _headers;

        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;

        public string GetHeader(string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName))
            {
                return string.Empty;
            }

            return _headers.TryGetValue(headerName, out var value) ? value : string.Empty;
        }
    }
}
