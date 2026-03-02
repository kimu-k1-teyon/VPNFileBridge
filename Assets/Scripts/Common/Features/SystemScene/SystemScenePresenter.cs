using Scripts.Common.Features.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Features.SystemScene
{
    public class SystemScenePresenter : IStartable
    {
        [Inject] SystemSceneModel _model;
        [Inject] SystemSceneView _view;
        [Inject] ISystemSceneService _service;

        [Inject] IConfigEnviourment _config;

        public void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            var enviourment = _config.GetEnviourment("Develop");
            Debug.Log(enviourment.APIConfig.BaseUrl);
            SceneManager.LoadScene("MainScene");
        }
    }
}
