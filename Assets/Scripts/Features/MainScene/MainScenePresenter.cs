using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.MainScene
{
    public class MainScenePresenter : IStartable
    {
        [Inject] private MainSceneModel _model;
        [Inject] private MainSceneView _view;
        [Inject] private IMainSceneService _service;

        public void Start()
        {
            // TODO: 初期化処理
        }
    }
}
