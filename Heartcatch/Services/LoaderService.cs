using System;
using System.Collections.Generic;
using Heartcatch.Models;
using UnityEngine;

namespace Heartcatch.Services
{
    public sealed class LoaderService : ILoaderService
    {
        private readonly Dictionary<string, AssetBundleModel> _assetBundles = new Dictionary<string, AssetBundleModel>()
            ;

        private readonly string _baseURL;

        private readonly Dictionary<string, AssetBundleModel> _loadingAssetBundles =
            new Dictionary<string, AssetBundleModel>();

        private readonly List<ILoadingOperation> _loadingOperations = new List<ILoadingOperation>();
        private AssetBundleManifest _assetBundleManifest;
        private readonly IAssetLoaderFactory _loaderFactory;

        public LoaderService(string url)
        {
            _baseURL = url;

#if LOCAL_BUNDLES
            var path = Path.Combine(Application.streamingAssetsPath,
                                    Path.Combine(Utility.ASSET_BUNDLES_OUTPUT_PATH, Utility.GetPlatformName()));
            Debug.LogFormat("Base path: {0}", path);
            _loaderFactory = new LocalAssetLoaderFactory(this, path);
#else
            Debug.LogFormat("Base URL: {0}", _baseURL);
            _loaderFactory = new WebAssetLoaderFactory(this, url);
#endif

            addLoadingOperation(_loaderFactory.LoadAssetBundleManifest(Utility.GetPlatformName()));
        }

        public bool IsLoading
        {
            get { return _loadingOperations.Count > 0; }
        }

        public bool IsInitialized
        {
            get { return _assetBundleManifest != null; }
        }

        public void LoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded)
        {
            var bundle = getAssetBundle(name);
            if (bundle.IsLoaded)
            {
                bundle.addReference();
                onLoaded(bundle);
            }
            else
            {
                if (!_loadingAssetBundles.ContainsKey(name))
                    loadAssetBundle(name);
                addLoadingOperation(new WaitForAssetBundleToLoad(this, name, onLoaded));
            }
        }

        public void GetOrLoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded)
        {
            var bundle = getAssetBundle(name);
            if (bundle.IsLoaded)
            {
                onLoaded(bundle);
            }
            else
            {
                if (!_loadingAssetBundles.ContainsKey(name))
                    loadAssetBundle(name);
                addLoadingOperation(new WaitForAssetBundleToLoad(this, name, onLoaded));
            }
        }

        public void UnloadAll()
        {
            foreach (var it in _assetBundles)
                it.Value.forceUnload();
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        internal IAssetBundleModel getLoadedAssetBundle(string name)
        {
            var bundle = getAssetBundle(name);
            if (bundle.IsLoaded)
                return bundle;
            throw new LoadingException(string.Format("Can't get asset bundle \"{0}\" - it isn't loaded yet", name));
        }

        internal void Update()
        {
            for (var i = 0; i < _loadingOperations.Count;)
            {
                var operation = _loadingOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    operation.Finish();
                    _loadingOperations.RemoveAt(i);
                }
            }
        }

        internal void onAssetBundleLoaded(string name, AssetBundle assetBundle)
        {
            AssetBundleModel bundle;
            if (_loadingAssetBundles.TryGetValue(name, out bundle))
            {
                bundle.onLoaded(assetBundle);
                _loadingAssetBundles.Remove(name);
            }
            else
            {
                throw new ArgumentException(string.Format("Asset bundle \"{0}\" isn't loading", name));
            }
        }

        internal void onManifestAssetBundleLoaded(AssetBundle assetBundle)
        {
            addLoadingOperation(new LoadAssetOperation<AssetBundleManifest>(assetBundle,
                "AssetBundleManifest",
                onAssetBundleManifestLoaded));
        }

        internal void onAssetBundleLoadFailed()
        {
        }

        internal bool isAssetBundleLoaded(string name)
        {
            return getAssetBundle(name).IsLoaded;
        }

        internal void addLoadingOperation(ILoadingOperation operation)
        {
            _loadingOperations.Add(operation);
        }

        private void onAssetBundleManifestLoaded(AssetBundleManifest manifest)
        {
            Debug.LogFormat("Manifest loaded: {0}", manifest);
            _assetBundleManifest = manifest;
            _assetBundles.Clear();
            foreach (var it in manifest.GetAllAssetBundles())
                _assetBundles.Add(it, new AssetBundleModel(this, manifest, it));
        }

        internal AssetBundleModel getAssetBundle(string name)
        {
            AssetBundleModel bundle;
            if (_assetBundles.TryGetValue(name, out bundle))
                return bundle;
            throw new ArgumentException(string.Format("Asset bundle \"{0}\" doesn't exist", name));
        }

        internal void loadAssetBundle(string name)
        {
            var bundle = getAssetBundle(name);
            if (bundle.IsLoadedItself || _loadingAssetBundles.ContainsKey(name))
                return;
            _loadingAssetBundles.Add(name, bundle);
            addLoadingOperation(_loaderFactory.LoadAssetBundle(name, _assetBundleManifest.GetAssetBundleHash(name)));
        }
    }
}