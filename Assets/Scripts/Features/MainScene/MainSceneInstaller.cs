using Scripts.Common.Features.RestApi;
using Scripts.Common.Log;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.MainScene
{
    public class MainSceneInstaller : LifetimeScope
    {
        [SerializeField] MainSceneView _view;
        [SerializeField] RestApiView _restApiView;
        protected override void Configure(IContainerBuilder builder)
        {
            LogInstaller.Register(builder);

            builder.Register<IMainSceneService, MainSceneServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(_view);
            builder.Register<MainSceneModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MainScenePresenter>();

            RestApiInstaller.Register(builder, _restApiView);
        }
    }
}
