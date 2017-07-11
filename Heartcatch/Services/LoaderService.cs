using System;
using System.Collections.Generic;
using Heartcatch.Models;
using UnityEngine;

namespace Heartcatch.Services
{
    public sealed class LoaderService : ILoaderService
    {
        private readonly Dictionary<string, AssetBundleModel> assetBundles = new Dictionary<string, AssetBundleModel>()
            ;

        private readonly string baseUrl;
        private readonly IAssetLoaderFactory loaderFactory;

        private readonly Dictionary<string, AssetBundleModel> loadingAssetBundles =
            new Dictionary<string, AssetBundleModel>();

        private readonly List<ILoadingOperation> loadingOperations = new List<ILoadingOperation>();
        private AssetBundleManifest assetBundleManifest;

        public LoaderService(string url)
        {
            baseUrl = url;

#if LOCAL_BUNDLES
            var path = Path.Combine(Application.streamingAssetsPath,
                                    Path.Combine(Utility.ASSET_BUNDLES_OUTPUT_PATH, Utility.GetPlatformName()));
            Debug.LogFormat("Base path: {0}", path);
            _loaderFactory = new LocalAssetLoaderFactory(this, path);
#else
            Debug.LogFormat("Base URL: {0}", baseUrl);
            loaderFactory = new WebAssetLoaderFactory(this, url);
#endif

            AddLoadingOperation(loaderFactory.LoadAssetBundleManifest(Utility.GetPlatformName()));
        }

        public bool IsLoading => loadingOperations.Count > 0;

        public bool IsInitialized => assetBundleManifest != null;

        public void LoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded)
        {
            var bundle = GetAssetBundle(name);
            if (bundle.IsLoaded)
            {
                bundle.AddReference();
                onLoaded(bundle);
            }
            else
            {
                if (!loadingAssetBundles.ContainsKey(name))
                    loadAssetBundle(name);
                AddLoadingOperation(new WaitForAssetBundleToLoad(this, name, onLoaded));
            }
        }

        public void GetOrLoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded)
        {
            var bundle = GetAssetBundle(name);
            if (bundle.IsLoaded)
            {
                onLoaded(bundle);
            }
            else
            {
                if (!loadingAssetBundles.ContainsKey(name))
                    loadAssetBundle(name);
                AddLoadingOperation(new WaitForAssetBundleToLoad(this, name, onLoaded));
            }
        }

        public void UnloadAll()
        {
            foreach (var it in assetBundles)
                it.Value.ForceUnload();
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        internal IAssetBundleModel GetLoadedAssetBundle(string name)
        {
            var bundle = GetAssetBundle(name);
            if (bundle.IsLoaded)
                return bundle;
            throw new LoadingException(string.Format("Can't get asset bundle \"{0}\" - it isn't loaded yet", name));
        }

        public void Update()
        {
            for (var i = 0; i < loadingOperations.Count;)
            {
                var operation = loadingOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    operation.Finish();
                    loadingOperations.RemoveAt(i);
                }
            }
        }

        internal void OnAssetBundleLoaded(string name, AssetBundle assetBundle)
        {
            AssetBundleModel bundle;
            if (loadingAssetBundles.TryGetValue(name, out bundle))
            {
                bundle.OnLoaded(assetBundle);
                loadingAssetBundles.Remove(name);
            }
            else
            {
                throw new ArgumentException(string.Format("Asset bundle \"{0}\" isn't loading", name));
            }
        }

        internal void OnManifestAssetBundleLoaded(AssetBundle assetBundle)
        {
            AddLoadingOperation(new LoadAssetOperation<AssetBundleManifest>(assetBundle,
                "AssetBundleManifest",
                OnAssetBundleManifestLoaded));
        }

        internal void OnAssetBundleLoadFailed()
        {
        }

        internal bool IsAssetBundleLoaded(string name)
        {
            return GetAssetBundle(name).IsLoaded;
        }

        internal void AddLoadingOperation(ILoadingOperation operation)
        {
            loadingOperations.Add(operation);
        }

        private void OnAssetBundleManifestLoaded(AssetBundleManifest manifest)
        {
            Debug.LogFormat("Manifest loaded: {0}", manifest);
            assetBundleManifest = manifest;
            assetBundles.Clear();
            foreach (var it in manifest.GetAllAssetBundles())
                assetBundles.Add(it, new AssetBundleModel(this, manifest, it));
        }

        internal AssetBundleModel GetAssetBundle(string name)
        {
            AssetBundleModel bundle;
            if (assetBundles.TryGetValue(name, out bundle))
                return bundle;
            throw new ArgumentException(string.Format("Asset bundle \"{0}\" doesn't exist", name));
        }

        internal void loadAssetBundle(string name)
        {
            var bundle = GetAssetBundle(name);
            if (bundle.IsLoadedItself || loadingAssetBundles.ContainsKey(name))
                return;
            loadingAssetBundles.Add(name, bundle);
            AddLoadingOperation(loaderFactory.LoadAssetBundle(name, assetBundleManifest.GetAssetBundleHash(name)));
        }
    }
}