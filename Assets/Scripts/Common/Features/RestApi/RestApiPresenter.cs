using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Scripts.Common.Features.Config;
using Scripts.Common.Log;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Features.RestApi
{
    public class RestApiPresenter : IStartable
    {
        [Inject] RestApiModel _model;
        [Inject] IConfigEnviourment _configEnviourment;
        [Inject] IRestApiUploadService _uploadService;
        [Inject] IRestApiDownloadService _downloadService;
        [Inject] IRestApiMasterService _masterService;
        [Inject] ILogService _log;

        public void Start()
        {
            Debug.Log(GetType().Name + "Started");
            _model.APIConfig = _configEnviourment.GetEnviourment("Develop").APIConfig;
            _model.BaseUrl = _configEnviourment.GetEnviourment("Develop").APIConfig.BaseUrl;
            InstanceClient(_model.APIConfig);

            _ = Post();
        }

        async Task Post()
        {
            // var result = await _downloadService.DownloadAsync("260316T1753");
            // if (result != null)
            // {
            //     Debug.Log($"Length: {result.Length}");
            // }
            var result = await _masterService.GetMasterData(1);
            if (result != null)
            {
                Debug.Log($"isSuccess: {result.isSuccess}");
                Debug.Log($"statusCode: {result.statusCode}");
                Debug.Log($"areaId: {result.areaId}");
                Debug.Log($"message: {result.message}");
                Debug.Log($"ms_areas: {result.ms_areas?.Length ?? 0}");
                Debug.Log($"ms_inspectors: {result.ms_inspectors?.Length ?? 0}");
                Debug.Log($"ms_inspection_targets: {result.ms_inspection_targets?.Length ?? 0}");

                if (result.ms_areas != null && result.ms_areas.Length > 0)
                {
                    Debug.Log($"Success: {result.ms_areas[0].area_name}");
                }
            }
            else
            {
                Debug.Log("Failed");
            }


            // var ct = new CancellationToken();
            // var filePath = Path.Combine(Application.streamingAssetsPath, "unity-sample-json.json");
            // var result = await _uploadService.UploadAsync("260319T1057", filePath, ct);

            // if (result != null)
            // {
            //     Debug.Log("result.StatusCode: " + result.StatusCode);
            // }


        }

        void InstanceClient(APIConfig config)
        {
            if (_model.Client == null)
            {
                // ハンドラの作成
                HttpClientHandler handler = new HttpClientHandler();

                if (_model.APIConfig.proxyAddress != null && _model.APIConfig.proxyAddress != "")
                {
                    handler.Proxy = new WebProxy(_model.APIConfig.proxyAddress);
                    handler.UseProxy = true;
                    // プロキシのログ出力
                    Debug.Log("GetProxy：" + handler.Proxy.GetProxy(new Uri(_model.APIConfig.BaseUrl)).ToString());
                }

                // HTTPCLIENTの作成
                _log.Write("BaseAddress: " + config.BaseUrl);
                _model.UploadTimeoutMS = Math.Max(config.TimeoutMS, _model.UploadTimeoutMS);
                _model.Client = new HttpClient(handler);
                _model.Client.Timeout = Timeout.InfiniteTimeSpan;
                _model.Client.BaseAddress = new Uri(config.BaseUrl);
            }
        }
    }
}
