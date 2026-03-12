using System;
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
        public string GetMasterDataEndpoint => string.Concat(BaseUrl, "/api/GetMasterData");
        public string LogDirectoryPath => Path.Combine(Application.persistentDataPath, "M3Logs");
        public string LogFilePath => Path.Combine(LogDirectoryPath, LogFileName);
    }

    public class Request
    {
        public string requestedPath;
        public string fileBase64;
    }

    public class MinimapData
    {

    }

    public class GetMasterDataResult
    {
        private GetMasterDataResult(
            bool isSuccess,
            long statusCode,
            int sosaKashoId,
            int targets,
            MasterDataItem[] sampleA,
            MasterDataItem[] sampleB,
            string message)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            SosaKashoId = sosaKashoId;
            Targets = targets;
            SampleA = sampleA ?? System.Array.Empty<MasterDataItem>();
            SampleB = sampleB ?? System.Array.Empty<MasterDataItem>();
            Message = message ?? string.Empty;
        }

        public bool IsSuccess { get; }
        public long StatusCode { get; }
        public int SosaKashoId { get; }
        public int Targets { get; }
        public MasterDataItem[] SampleA { get; }
        public MasterDataItem[] SampleB { get; }
        public string Message { get; }

        public static GetMasterDataResult Success(
            long statusCode,
            int sosaKashoId,
            int targets,
            MasterDataItem[] sampleA,
            MasterDataItem[] sampleB)
        {
            return new GetMasterDataResult(true, statusCode, sosaKashoId, targets, sampleA, sampleB, string.Empty);
        }

        public static GetMasterDataResult Failure(long statusCode, string message)
        {
            return new GetMasterDataResult(false, statusCode, 0, 0, System.Array.Empty<MasterDataItem>(), System.Array.Empty<MasterDataItem>(), message);
        }
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
}
