using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Common.Features.RestApi
{
    public static class RestApiInstaller
    {
        public static void Register(IContainerBuilder builder, RestApiView view)
        {
            builder.Register<IRestApiService, RestApiServiceImpl>(Lifetime.Singleton);
            builder.Register<IRestApiMasterService, RestApiMasterServiceImpl>(Lifetime.Singleton);

            builder.Register<IRestApiUploadService, RestApiUploadServiceImpl>(Lifetime.Singleton);
            builder.Register<IRestApiDownloadService, RestApiDownloadServiceImpl>(Lifetime.Singleton);
            builder.RegisterComponent(view);
            builder.Register<RestApiModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<RestApiPresenter>();
        }
    }
}
