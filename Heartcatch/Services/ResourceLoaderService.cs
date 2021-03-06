﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heartcatch.Models;
using strange.extensions.pool.api;
using UnityEngine;

namespace Heartcatch.Services
{
    public sealed class ResourceLoaderService : IResourceLoaderService
    {
        [Inject]
        public ILoaderService LoaderService { get; set; }

        [Inject]
        public IPool<ResourceRequestModel> ResourceRequests { get; set; }

        public void RequestResources(IResourceModel resourceModel)
        {
            var requestModel = ResourceRequests.GetInstance();
            resourceModel.CollectResources(requestModel);
            foreach (var request in requestModel)
            {
                var name = request.Key;
                var assetBundle = request.Value.AssetBundle;
                var assetName = request.Value.AssetName;
                LoaderService.GetOrLoadAssetBundle(assetBundle, bundle =>
                {
                    bundle.LoadAsset<Object>(assetName, resource =>
                    {
                        requestModel.OnResourceLoaded(name, resource);
                        if (requestModel.IsAllResourcesLoaded())
                        {
                            resourceModel.OnResourcesLoaded(requestModel);
                            requestModel.Release();
                            ResourceRequests.ReturnInstance(requestModel);
                        }
                    });
                });
            }
        }
    }
}
