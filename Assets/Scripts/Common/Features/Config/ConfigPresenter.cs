using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Features.Config
{
    public class ConfigPresenter : IStartable
    {
        [Inject] private ConfigModel _model;
        [Inject] private ConfigView _view;
        [Inject] private IConfigService _service;

        public void Start()
        {
            // TODO: 初期化処理
        }
    }
}
