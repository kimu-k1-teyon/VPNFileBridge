using Scripts.Common.Features.Config;
using Scripts.Common.Features.RestApi;
using Scripts.Common.Log;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Features.SystemScene
{
    public class SystemSceneInstaller : LifetimeScope
    {
        [SerializeField] SystemSceneView _view;
        [SerializeField] ConfigView _configView;
        [SerializeField] RestApiView _restApiView;

        protected override void Configure(IContainerBuilder builder)
        {
            ConfigInstaller.Register(builder, _configView);
            RestApiInstaller.Register(builder, _restApiView);
            LogInstaller.Register(builder);

            builder.Register<ISystemSceneService, SystemSceneServiceImpl>(Lifetime.Singleton);

            builder.RegisterComponent(_view);
            builder.Register<SystemSceneModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SystemScenePresenter>();
        }
    }
}
