using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Features.MainScene
{
    public class MainSceneInstaller : LifetimeScope
    {
        [SerializeField] MainSceneView _view;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IMainSceneService, MainSceneServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(_view);
            builder.Register<MainSceneModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MainScenePresenter>();
        }
    }
}
