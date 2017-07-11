using System;
using Heartcatch.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heartcatch.Models
{
    public sealed class AssetBundleModel : IAssetBundleModel
    {
        private readonly string[] _dependencies;
        private readonly LoaderService _loaderService;
        private readonly string _name;
        private AssetBundle _assetBundle;
        private int _referenceCount;

        public AssetBundleModel(LoaderService loaderService, AssetBundleManifest manifest, string name)
        {
            _name = name;
            _loaderService = loaderService;
            _referenceCount = 0;
            _dependencies = manifest.GetDirectDependencies(name);
        }

        public bool IsLoaded
        {
            get { return IsLoadedItself && isAllDependenciesLoaded(); }
        }

        public bool IsLoadedItself
        {
            get { return _assetBundle != null; }
        }

        public void LoadAsset<T>(string name, Action<T> onLoaded) where T : Object
        {
            checkIfLoaded();
            _loaderService.addLoadingOperation(new LoadAssetOperation<T>(_assetBundle, name, onLoaded));
        }

        public void LoadAllAssets<T>(Action<T[]> onLoaded) where T : Object
        {
            checkIfLoaded();
            _loaderService.addLoadingOperation(new LoadAllAssetsOperation<T>(_assetBundle, onLoaded));
        }

        public string GetScenePath(int index)
        {
            checkIfLoaded();
            var allPaths = _assetBundle.GetAllScenePaths();
            return allPaths[index];
        }

        public void Unload()
        {
            _referenceCount--;
            if (_referenceCount == 0)
            {
                _assetBundle.Unload(true);
                _assetBundle = null;
                foreach (var it in _dependencies)
                    _loaderService.getAssetBundle(it).Unload();
            }
        }

        internal void onLoaded(AssetBundle assetBundle)
        {
            if (_assetBundle != null || _referenceCount != 0)
                throw new InvalidOperationException(string.Format("AssetBundleModel \"{0}\" is already loaded", _name));
            _assetBundle = assetBundle;
            _referenceCount = 1;
            foreach (var it in _dependencies)
                _loaderService.loadAssetBundle(it);
        }

        internal void addReference()
        {
            if (_assetBundle == null)
                throw new InvalidOperationException(
                    string.Format("Can add reference to not loaded asset bundle \"0\"", _name));
            _referenceCount++;
        }

        private void checkIfLoaded()
        {
            if (!IsLoaded)
                throw new LoadingException(string.Format("Asset bundle \"{0}\" isn't loaded yet", _name));
        }

        internal void forceUnload()
        {
            _referenceCount = 0;
            if (_assetBundle != null)
            {
                _assetBundle.Unload(true);
                _assetBundle = null;
            }
        }

        private bool isAllDependenciesLoaded()
        {
            foreach (var it in _dependencies)
                if (!_loaderService.isAssetBundleLoaded(it))
                    return false;
            return true;
        }
    }
}