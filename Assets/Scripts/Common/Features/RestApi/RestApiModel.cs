using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using Scripts.Common.Features.Config;
using UnityEngine;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiModel
    {
        public HttpClient Client { get; set; }
        public APIConfig APIConfig { get; set; }

        public string BaseUrl { get; set; }
        public string LogFileName { get; set; }

        public string HealthEndpoint => string.Concat(BaseUrl, "/api/Health");
        public string UploadEndpoint => string.Concat(BaseUrl, "/api/Upload");
        public string DownloadEndpoint => string.Concat(BaseUrl, "/api/Download");
        public string GetMasterDataEndpoint => string.Concat(BaseUrl, "/api/GetMasterData");
        public string LogDirectoryPath => Path.Combine(Application.persistentDataPath, "M3Logs");
        public string LogFilePath => Path.Combine(LogDirectoryPath, LogFileName);
    }

    [Serializable]
    public class GetMasterDataResult
    {
        public bool isSuccess;
        public long statusCode;
        public int sosaKashoId;
        public int targets;
        public MasterDataItem[] sampleA;
        public MasterDataItem[] sampleB;
        public string message;
    }

    [Serializable]
    public class MasterDataItem
    {
        public string id;
        public string value0;
        public string value1;
    }

    [Serializable]
    public class GetMasterDataRequestDto
    {
        public int targets;
        public int sosaKashoId;
    }

    [Serializable]
    public class GetMasterDataResponseDto
    {
        public int sosaKashoId;
        public int targets;
        public MasterDataItem[] sampleA;
        public MasterDataItem[] sampleB;
    }

    public class UploadResult
    {
        private UploadResult(bool isSuccess, long statusCode, string uploadId, string message)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            UploadId = uploadId ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public bool IsSuccess { get; }
        public long StatusCode { get; }
        public string UploadId { get; }
        public string Message { get; }

        public static UploadResult Success(long statusCode, string uploadId)
        {
            return new UploadResult(true, statusCode, uploadId, string.Empty);
        }

        public static UploadResult Failure(long statusCode, string message)
        {
            return new UploadResult(false, statusCode, string.Empty, message);
        }
    }

    [Serializable]
    public class DownloadResult
    {
        public bool isSuccess;
        public long statusCode;
        public string fileName;
        public byte[] fileBytes;
        public string message;
    }

    [Serializable]
    public class UploadResponseDto
    {
        public string status;
        public string uploadId;
        public string message;
    }

    [Serializable]
    public class ApiErrorResponseDto
    {
        public string status;
        public string message;
    }

    [Serializable]
    public class DownloadRequestDto
    {
        public string uploadId;
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
