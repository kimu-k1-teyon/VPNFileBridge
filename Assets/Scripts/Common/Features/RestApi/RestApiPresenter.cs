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
        [Inject] RestApiView _view;
        [Inject] IRestApiService _service;
        [Inject] IRestApiMasterService _masterService;
        [Inject] IConfigEnviourment _configEnviourment;
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
            var ct = new CancellationToken();
            var health = await _service.GetAsync(_model.HealthEndpoint, ct);
            if (health != null)
            {
                _log.Write("health.IsSuccessStatusCode: " + health.IsSuccessStatusCode);
                Debug.Log("health.IsSuccessStatusCode: " + health.IsSuccessStatusCode);

                if (health.IsSuccessStatusCode)
                {
                    var targets = 11;
                    var sosaKashoId = 2;
                    _log.Write($"targets: {targets}, sosaKashoId: {sosaKashoId}");
                    var result = await _masterService.GetAsync(targets, sosaKashoId, ct);
                    _log.Write("result.IsSuccess: " + result.IsSuccess);
                    Debug.Log("result.IsSuccess: " + result.IsSuccess);

                    if (result.IsSuccess)
                    {
                        _log.Write("result.SampleA.Length: " + result.SampleA.Length);
                        _log.Write("result.SampleB.Length: " + result.SampleB.Length);
                    }

                }
                else
                {
                    _log.Write("NG result is null");
                }
            }
            else
            {
                _log.Write("NG health");
            }
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
