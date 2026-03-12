using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiUploadServiceImpl : IRestApiUploadService
    {
        [Inject] IRestApiService _service;
        [Inject] RestApiModel _model;
        const int MaxUploadBytes = 10 * 1024 * 1024;

        public async Task<UploadResult> UploadAsync(string filePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return UploadResult.Failure(0, "File path is empty.");
            }

            if (!File.Exists(filePath))
            {
                return UploadResult.Failure(0, string.Concat("File not found: ", filePath));
            }

            var fileBytes = File.ReadAllBytes(filePath);
            if (fileBytes.Length > MaxUploadBytes)
            {
                return UploadResult.Failure(413, "File must be 10MB or smaller.");
            }

            var metaJson = "{\"clientRequestId\":\"sample-001\"}";
            var response = await _service.PostMultipartAsync(
                _model.UploadEndpoint,
                fileBytes,
                Path.GetFileName(filePath),
                metaJson,
                cancellationToken);

            if (!response.IsTransportSuccess)
            {
                return UploadResult.Failure(response.StatusCode, response.ErrorMessage);
            }

            if (!response.IsSuccessStatusCode)
            {
                return UploadResult.Failure(response.StatusCode, "Failure");
            }

            if (string.IsNullOrWhiteSpace(response.ResponseText))
            {
                return UploadResult.Failure(response.StatusCode, "Empty response body.");
            }

            var dto = JsonUtility.FromJson<UploadResponseDto>(response.ResponseText);
            if (dto == null || !string.Equals(dto.status, "ok", StringComparison.OrdinalIgnoreCase))
            {
                return UploadResult.Failure(response.StatusCode, dto != null ? dto.message : "Upload failed.");
            }

            return UploadResult.Success(response.StatusCode, dto.uploadId);
        }
    }
}
