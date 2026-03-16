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
            var result = await _downloadService.DownloadAsync("abcd-unity");
            if (result != null)
            {
                Debug.Log($"Length: {result.Length}");
            }
            // var result = await _masterService.GetMasterData(10, 1);
            // if (result != null)
            // {
            //     Debug.Log($"isSuccess: {result.isSuccess}");
            //     Debug.Log($"statusCode: {result.statusCode}");
            //     Debug.Log($"targets: {result.targets}");
            //     Debug.Log($"sosaKashoId: {result.sosaKashoId}");
            //     Debug.Log($"message: {result.message}");

            //     Debug.Log($"Success: {result.sampleB[0].id}");
            // }
            // else
            // {
            //     Debug.Log("Failed");
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
                _model.Client = new HttpClient(handler);
                _model.Client.Timeout = TimeSpan.FromMilliseconds(config.TimeoutMS);
                _model.Client.BaseAddress = new Uri(config.BaseUrl);
            }
        }
    }
}
