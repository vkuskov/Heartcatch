using System;
using Heartcatch.Core.Models;
using strange.extensions.injector.api;
using strange.extensions.pool.api;
using Object = UnityEngine.Object;

namespace Heartcatch.Core.Services
{
    public sealed class ResourceLoaderService : IResourceLoaderService
    {
        [Inject]
        public IAssetLoaderService AssetLoaderService { get; set; }

        [Inject]
        public IInjectionBinder InjectionBinder { get; set; }

        public void RequestResources(IResourceModel resourceModel, Action<IResourceRequestModel> onLoaded)
        {
            var requestModel = new ResourceRequestModel();
            resourceModel.CollectResources(requestModel);
            foreach (var request in requestModel)
            {
                var name = request.Key;
                var assetBundle = request.Value.AssetBundle;
                var assetName = request.Value.AssetName;
                AssetLoaderService.LoadAssetBundle(assetBundle, bundle =>
                {
                    bundle.LoadAsset<Object>(assetName, resource =>
                    {
                        requestModel.OnResourceLoaded(name, resource);
                        if (requestModel.IsAllResourcesLoaded())
                        {
                            onLoaded(requestModel);
                        }
                    });
                });
            }
        }
    }
}