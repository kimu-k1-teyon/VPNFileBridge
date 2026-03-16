using System;
using System.IO;
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
            var id = "abcd-unity";
            var filePath = Path.Combine(Application.streamingAssetsPath, "unity-sample-json.json");
            var ct = new CancellationToken();
            var result = await _uploadService.UploadAsync(id, filePath, ct);
            if (result.IsSuccess)
            {
                Debug.Log(result.StatusCode);
                Debug.Log(result.Message);
            }
            else
            {
                Debug.Log("Failed");
            }
            // throw new NotImplementedException();
        }

        void InstanceClient(APIConfig config)
        {
            if (_model.Client == null)
            {
                // ハンドラの作成
                HttpClientHandler handler = new HttpClientHandler();

                // HTTPCLIENTの作成
                _log.Write("BaseAddress: " + config.BaseUrl);
                _model.Client = new HttpClient(handler);
                _model.Client.Timeout = TimeSpan.FromMilliseconds(config.TimeoutMS);
                _model.Client.BaseAddress = new Uri(config.BaseUrl);
            }
        }
    }
}
