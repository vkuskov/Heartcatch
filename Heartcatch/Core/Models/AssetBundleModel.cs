using System;
using Heartcatch.Core.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heartcatch.Core.Models
{
    public sealed class AssetBundleModel : IAssetBundleModel
    {
        private readonly BaseLoaderService baseLoaderService;
        private readonly string[] dependencies;
        private readonly string name;
        private AssetBundle assetBundle;

        public AssetBundleModel(BaseLoaderService baseLoaderService, AssetBundleManifest manifest, string name)
        {
            this.name = name;
            this.baseLoaderService = baseLoaderService;
            dependencies = manifest.GetDirectDependencies(name);
        }

        public bool IsLoaded
        {
            get { return IsLoadedItself && IsAllDependenciesLoaded(); }
        }

        public bool IsLoadedItself
        {
            get { return assetBundle != null; }
        }

        public void LoadAsset<T>(string name, Action<T> onLoaded) where T : Object
        {
            CheckIfLoaded();
            baseLoaderService.AddLoadingOperation(new LoadAssetOperation<T>(assetBundle, name, onLoaded));
        }

        public void LoadAllAssets<T>(Action<T[]> onLoaded) where T : Object
        {
            CheckIfLoaded();
            baseLoaderService.AddLoadingOperation(new LoadAllAssetsOperation<T>(assetBundle, onLoaded));
        }

        public string GetScenePath(int index)
        {
            CheckIfLoaded();
            var allPaths = assetBundle.GetAllScenePaths();
            return allPaths[index];
        }

        public void Unload()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
        }

        internal void OnLoaded(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            foreach (var it in dependencies)
                baseLoaderService.LoadAssetBundle(it);
        }

        private void CheckIfLoaded()
        {
            if (!IsLoaded)
                throw new LoadingException(string.Format("Asset bundle \"{0}\" isn't loaded yet", name));
        }

        private bool IsAllDependenciesLoaded()
        {
            foreach (var it in dependencies)
                if (!baseLoaderService.IsAssetBundleLoaded(it))
                    return false;
            return true;
        }
    }
}