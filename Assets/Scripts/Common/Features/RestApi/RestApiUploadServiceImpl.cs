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

        public async Task<UploadResult> UploadAsync(string uploadId, string filePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(uploadId))
            {
                return UploadResult.Failure(400, "uploadId is empty.");
            }

            if (!IsValidUploadId(uploadId))
            {
                return UploadResult.Failure(400, "uploadId must be 1-64 characters of A-Z, a-z, 0-9, underscore, or hyphen.");
            }

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

            var response = await _service.PostMultipartAsync(
                _model.UploadEndpoint,
                uploadId,
                fileBytes,
                Path.GetFileName(filePath),
                cancellationToken);

            if (!response.IsTransportSuccess)
            {
                return UploadResult.Failure(response.StatusCode, response.ErrorMessage);
            }

            if (!response.IsSuccessStatusCode)
            {
                return UploadResult.Failure(response.StatusCode, GetErrorMessage(response));
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

        static string GetErrorMessage(ApiHttpResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.ResponseText))
            {
                var errorDto = JsonUtility.FromJson<ApiErrorResponseDto>(response.ResponseText);
                if (errorDto != null && !string.IsNullOrWhiteSpace(errorDto.message))
                {
                    return errorDto.message;
                }
            }

            return "Failure";
        }

        static bool IsValidUploadId(string uploadId)
        {
            if (uploadId.Length > 64)
            {
                return false;
            }

            foreach (var ch in uploadId)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-')
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}
