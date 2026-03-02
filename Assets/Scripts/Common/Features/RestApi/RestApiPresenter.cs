using System;
using System.IO;
using System.Net.Http;
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
        [Inject] IConfigEnviourment _configEnviourment;
        [Inject] ILogService _log;

        public void Start()
        {
            Debug.Log(GetType().Name + "Started");
            _model.APIConfig = _configEnviourment.GetEnviourment("Develop").APIConfig;
            InstanceClient(_model.APIConfig);
            _ = Post();
        }

        async Task Post()
        {
            // var request = new Request();
            // request.Target = "documents/from-unity.json";
            // request.ID = "eyJoZWxsbyI6InVuaXR5In0=";
            // var json = JsonUtility.ToJson(request);
            // var result = await _service.PostAsync(json, @"api/UploadFile");

            var result = await _service.PostAsync2();
            if (result != null)
            {
                _log.Write("Result: " + result);
            }
            else
            {
                _log.Write("Result is null");
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
