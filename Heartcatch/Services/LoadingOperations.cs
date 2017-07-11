using System;
using System.IO;
using Heartcatch.Models;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Heartcatch.Services
{
    internal interface ILoadingOperation
    {
        bool Update();
        void Finish();
    }

    internal sealed class WaitForAssetBundleToLoad : ILoadingOperation
    {
        private readonly LoaderService _loaderService;
        private readonly string _name;
        private readonly Action<IAssetBundleModel> _onLoaded;

        public WaitForAssetBundleToLoad(LoaderService loaderService, string name, Action<IAssetBundleModel> onLoaded)
        {
            _loaderService = loaderService;
            _name = name;
            _onLoaded = onLoaded;
        }

        public bool Update()
        {
            return !_loaderService.isAssetBundleLoaded(_name);
        }

        public void Finish()
        {
            _onLoaded(_loaderService.getLoadedAssetBundle(_name));
        }
    }

    internal abstract class BaseAssetBundleLoadOperation : ILoadingOperation
    {
        private readonly AssetBundleCreateRequest _createRequest;
        protected readonly LoaderService _loaderService;

        protected BaseAssetBundleLoadOperation(LoaderService loaderService, string path)
        {
            _loaderService = loaderService;
            _createRequest = AssetBundle.LoadFromFileAsync(path);
        }

        public bool Update()
        {
            return !_createRequest.isDone;
        }

        public void Finish()
        {
            if (!_createRequest.isDone)
                throw new InvalidOperationException("Can't finish load operation that is in progress");
            onLoaded(_createRequest.assetBundle);
        }

        protected abstract void onLoaded(AssetBundle assetBundle);
    }

    internal abstract class BaseAssetBundleDownloadOperation : ILoadingOperation
    {
        protected readonly LoaderService _loaderService;
        private readonly UnityWebRequest _request;

        protected BaseAssetBundleDownloadOperation(LoaderService loaderService, string downloadURL)
        {
            _loaderService = loaderService;
            _request = UnityWebRequest.GetAssetBundle(downloadURL);
            _request.Send();
        }

        protected BaseAssetBundleDownloadOperation(LoaderService loaderService, string downloadURL, Hash128 hash)
        {
            _loaderService = loaderService;
            _request = UnityWebRequest.GetAssetBundle(downloadURL, hash, 0);
            _request.Send();
        }

        public void Finish()
        {
            if (!_request.isDone)
                throw new InvalidOperationException("Can't finish download operation that is in progress");
            if (_request.isNetworkError)
            {
                Debug.LogWarningFormat("Failed to download asset bundle from {0}", _request.url);
                onFailed();
            }
            else
            {
                onLoaded(DownloadHandlerAssetBundle.GetContent(_request));
            }
            _request.Dispose();
        }

        public bool Update()
        {
            return !_request.isDone;
        }

        protected abstract void onLoaded(AssetBundle assetBundle);

        private void onFailed()
        {
            _loaderService.onAssetBundleLoadFailed();
        }
    }

    internal sealed class AssetBundleManifestDownloadOperation : BaseAssetBundleDownloadOperation
    {
        public AssetBundleManifestDownloadOperation(LoaderService loaderService, string downloadURL) : base(
            loaderService,
            downloadURL)
        {
        }

        protected override void onLoaded(AssetBundle assetBundle)
        {
            _loaderService.onManifestAssetBundleLoaded(assetBundle);
        }
    }

    internal sealed class AssetBundleManifestLoadOperation : BaseAssetBundleLoadOperation
    {
        public AssetBundleManifestLoadOperation(LoaderService loaderService, string path) :
            base(loaderService, path)
        {
        }

        protected override void onLoaded(AssetBundle assetBundle)
        {
            _loaderService.onManifestAssetBundleLoaded(assetBundle);
        }
    }

    internal sealed class AssetBundleDownloadOperation : BaseAssetBundleDownloadOperation
    {
        private readonly string _assetBundleName;

        public AssetBundleDownloadOperation(LoaderService loaderService,
            string assetBundleName,
            string downloadURL) : base(loaderService, downloadURL)
        {
            _assetBundleName = assetBundleName;
        }

        public AssetBundleDownloadOperation(LoaderService loaderService,
            string assetBundleName,
            string downloadURL,
            Hash128 hash) : base(
            loaderService,
            downloadURL,
            hash)
        {
            _assetBundleName = assetBundleName;
        }

        protected override void onLoaded(AssetBundle assetBundle)
        {
            _loaderService.onAssetBundleLoaded(_assetBundleName, assetBundle);
        }
    }

    internal sealed class AssetBundleLoadOperation : BaseAssetBundleLoadOperation
    {
        private readonly string _assetBundleName;

        public AssetBundleLoadOperation(LoaderService loaderService, string path) : base(loaderService, path)
        {
            _assetBundleName = Path.GetFileNameWithoutExtension(path);
        }

        protected override void onLoaded(AssetBundle assetBundle)
        {
            _loaderService.onAssetBundleLoaded(_assetBundleName, assetBundle);
        }
    }

    internal abstract class BaseAssetLoadOperation : ILoadingOperation
    {
        protected readonly AssetBundleRequest _request;

        protected BaseAssetLoadOperation(AssetBundleRequest request)
        {
            _request = request;
        }

        public void Finish()
        {
            if (!_request.isDone)
                throw new InvalidOperationException("Can't finish loading operation that isn't done");
            onFinish(_request);
        }

        public bool Update()
        {
            return !_request.isDone;
        }

        protected abstract void onFinish(AssetBundleRequest request);
    }

    internal sealed class LoadAssetOperation<T> : BaseAssetLoadOperation where T : Object
    {
        private readonly Action<T> _onLoaded;

        public LoadAssetOperation(AssetBundle assetBundle, string name, Action<T> onLoaded) : base(assetBundle
            .LoadAssetAsync(
                name))
        {
            _onLoaded = onLoaded;
        }

        protected override void onFinish(AssetBundleRequest request)
        {
            _onLoaded(request.asset as T);
        }
    }

    internal sealed class LoadAllAssetsOperation<T> : BaseAssetLoadOperation where T : Object
    {
        private readonly Action<T[]> _onLoaded;

        public LoadAllAssetsOperation(AssetBundle assetBundle, Action<T[]> onLoaded) : base(assetBundle
            .LoadAllAssetsAsync<T
            >())
        {
            _onLoaded = onLoaded;
        }

        protected override void onFinish(AssetBundleRequest request)
        {
            var loadedAssets = request.allAssets;
            var result = new T[loadedAssets.Length];
            for (var i = 0; i < loadedAssets.Length; i++)
                result[i] = loadedAssets[i] as T;
            _onLoaded(result);
        }
    }

    internal interface IAssetLoaderFactory
    {
        ILoadingOperation LoadAssetBundleManifest(string assetBundleName);
        ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash);
    }

    internal sealed class WebAssetLoaderFactory : IAssetLoaderFactory
    {
        private readonly string _baseURL;
        private readonly LoaderService _loaderService;

        public WebAssetLoaderFactory(LoaderService loaderService, string baseURL)
        {
            _loaderService = loaderService;
            _baseURL = baseURL;
        }

        public ILoadingOperation LoadAssetBundleManifest(string assetBundleName)
        {
            return new AssetBundleManifestDownloadOperation(_loaderService, getURLForBundle(assetBundleName));
        }

        public ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash)
        {
            return new AssetBundleDownloadOperation(_loaderService,
                assetBundleName,
                getURLForBundle(assetBundleName),
                hash);
        }

        private string getURLForBundle(string assetBundleName)
        {
            return string.Format("{0}/{1}", _baseURL, assetBundleName);
        }
    }

    internal sealed class LocalAssetLoaderFactory : IAssetLoaderFactory
    {
        private readonly LoaderService _loaderService;
        private readonly string _rootPath;

        public LocalAssetLoaderFactory(LoaderService loaderService, string rootPath)
        {
            _loaderService = loaderService;
            _rootPath = rootPath;
        }

        public ILoadingOperation LoadAssetBundleManifest(string assetBundleName)
        {
            return new AssetBundleManifestLoadOperation(_loaderService, getBundlePath(assetBundleName));
        }

        public ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash)
        {
            return new AssetBundleLoadOperation(_loaderService, getBundlePath(assetBundleName));
        }

        private string getBundlePath(string assetBundleName)
        {
            return Path.Combine(_rootPath, assetBundleName);
        }
    }
}