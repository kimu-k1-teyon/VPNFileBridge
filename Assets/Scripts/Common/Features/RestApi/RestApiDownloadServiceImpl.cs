using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiDownloadServiceImpl : IRestApiDownloadService
    {
        [Inject] IRestApiService _service;
        [Inject] RestApiModel _model;

        public async Task<DownloadResult> DownloadAsync(string uploadId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(uploadId))
            {
                return DownloadResult.Failure(400, "uploadId is empty.");
            }

            var payload = new DownloadRequestDto
            {
                uploadId = uploadId,
            };
            var response = await _service.PostJsonAsync(
                _model.DownloadEndpoint,
                JsonUtility.ToJson(payload),
                cancellationToken);

            if (!response.IsTransportSuccess)
            {
                return DownloadResult.Failure(response.StatusCode, response.ErrorMessage);
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = TryGetErrorMessage(response.ResponseText);
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Download failed.";
                }

                return DownloadResult.Failure(response.StatusCode, message);
            }

            var fileName = TryParseFileName(response.GetHeader("Content-Disposition"));
            return DownloadResult.Success(response.StatusCode, fileName, response.ResponseBytes);
        }

        static string TryGetErrorMessage(string responseText)
        {
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return string.Empty;
            }

            try
            {
                var dto = JsonUtility.FromJson<ApiErrorResponseDto>(responseText);
                return dto != null && !string.IsNullOrWhiteSpace(dto.message)
                    ? dto.message
                    : responseText;
            }
            catch (ArgumentException)
            {
                return responseText;
            }
        }

        static string TryParseFileName(string contentDisposition)
        {
            if (string.IsNullOrWhiteSpace(contentDisposition))
            {
                return "download.bin";
            }

            var utf8Marker = "filename*=UTF-8''";
            var utf8Index = contentDisposition.IndexOf(utf8Marker, StringComparison.OrdinalIgnoreCase);
            if (utf8Index >= 0)
            {
                var encodedValue = contentDisposition.Substring(utf8Index + utf8Marker.Length);
                var delimiterIndex = encodedValue.IndexOf(';');
                if (delimiterIndex >= 0)
                {
                    encodedValue = encodedValue.Substring(0, delimiterIndex);
                }

                encodedValue = encodedValue.Trim().Trim('"');
                if (!string.IsNullOrWhiteSpace(encodedValue))
                {
                    return Uri.UnescapeDataString(encodedValue);
                }
            }

            var fileNameMarker = "filename=";
            var fileNameIndex = contentDisposition.IndexOf(fileNameMarker, StringComparison.OrdinalIgnoreCase);
            if (fileNameIndex >= 0)
            {
                var rawValue = contentDisposition.Substring(fileNameIndex + fileNameMarker.Length);
                var delimiterIndex = rawValue.IndexOf(';');
                if (delimiterIndex >= 0)
                {
                    rawValue = rawValue.Substring(0, delimiterIndex);
                }

                rawValue = rawValue.Trim().Trim('"');
                if (!string.IsNullOrWhiteSpace(rawValue))
                {
                    return rawValue;
                }
            }

            return "download.bin";
        }
    }
}
