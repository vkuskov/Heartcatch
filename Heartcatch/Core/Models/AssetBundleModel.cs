using System;
using Heartcatch.Core.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heartcatch.Core.Models
{
    public sealed class AssetBundleModel : IAssetBundleModel
    {
        private readonly string[] dependencies;
        private readonly BaseLoaderService baseLoaderService;
        private readonly string name;
        private AssetBundle assetBundle;
        private int referenceCount;

        public AssetBundleModel(BaseLoaderService baseLoaderService, AssetBundleManifest manifest, string name)
        {
            this.name = name;
            this.baseLoaderService = baseLoaderService;
            referenceCount = 0;
            dependencies = manifest.GetDirectDependencies(name);
        }

        public bool IsLoaded => IsLoadedItself && IsAllDependenciesLoaded();

        public bool IsLoadedItself => assetBundle != null;

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
            referenceCount--;
            if (referenceCount == 0)
            {
                assetBundle.Unload(true);
                assetBundle = null;
                foreach (var it in dependencies)
                    baseLoaderService.GetAssetBundle(it).Unload();
            }
        }

        internal void OnLoaded(AssetBundle assetBundle)
        {
            if (this.assetBundle != null || referenceCount != 0)
                throw new InvalidOperationException(string.Format("AssetBundleModel \"{0}\" is already loaded", name));
            this.assetBundle = assetBundle;
            referenceCount = 1;
            foreach (var it in dependencies)
                baseLoaderService.loadAssetBundle(it);
        }

        internal void AddReference()
        {
            if (assetBundle == null)
                throw new InvalidOperationException(
                    string.Format("Can add reference to not loaded asset bundle \"0\"", name));
            referenceCount++;
        }

        private void CheckIfLoaded()
        {
            if (!IsLoaded)
                throw new LoadingException(string.Format("Asset bundle \"{0}\" isn't loaded yet", name));
        }

        internal void ForceUnload()
        {
            referenceCount = 0;
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
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