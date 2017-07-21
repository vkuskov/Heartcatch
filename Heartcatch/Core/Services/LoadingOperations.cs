using System;
using System.Collections.Generic;
using System.IO;
using Heartcatch.Core.Models;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Heartcatch.Core.Services
{
    public interface ILoadingOperation
    {
        bool Update();
        void Finish();
    }

    internal sealed class WaitForAssetBundleToLoad : ILoadingOperation
    {
        private readonly BaseAssetLoaderService baseAssetLoaderService;
        private readonly string name;
        private readonly Action<IAssetBundleModel> onLoaded;

        public WaitForAssetBundleToLoad(BaseAssetLoaderService baseAssetLoaderService, string name,
            Action<IAssetBundleModel> onLoaded)
        {
            this.baseAssetLoaderService = baseAssetLoaderService;
            this.name = name;
            this.onLoaded = onLoaded;
        }

        public bool Update()
        {
            return !baseAssetLoaderService.IsAssetBundleLoaded(name);
        }

        public void Finish()
        {
            onLoaded(baseAssetLoaderService.GetLoadedAssetBundle(name));
        }
    }

    internal abstract class BaseAssetBundleLoadOperation : ILoadingOperation
    {
        protected readonly BaseAssetLoaderService BaseAssetLoaderService;
        private readonly AssetBundleCreateRequest createRequest;

        protected BaseAssetBundleLoadOperation(BaseAssetLoaderService baseAssetLoaderService, string path)
        {
            BaseAssetLoaderService = baseAssetLoaderService;
            createRequest = AssetBundle.LoadFromFileAsync(path);
        }

        public bool Update()
        {
            return !createRequest.isDone;
        }

        public void Finish()
        {
            if (!createRequest.isDone)
                throw new InvalidOperationException("Can't finish load operation that is in progress");
            OnLoaded(createRequest.assetBundle);
        }

        protected abstract void OnLoaded(AssetBundle assetBundle);
    }

    internal abstract class BaseAssetBundleDownloadOperation : ILoadingOperation
    {
        protected readonly BaseAssetLoaderService BaseAssetLoaderService;
        private readonly UnityWebRequest request;

        protected BaseAssetBundleDownloadOperation(BaseAssetLoaderService baseAssetLoaderService, string downloadUrl)
        {
            BaseAssetLoaderService = baseAssetLoaderService;
            request = UnityWebRequest.GetAssetBundle(downloadUrl);
            request.Send();
        }

        protected BaseAssetBundleDownloadOperation(BaseAssetLoaderService baseAssetLoaderService, string downloadUrl,
            Hash128 hash)
        {
            BaseAssetLoaderService = baseAssetLoaderService;
            request = UnityWebRequest.GetAssetBundle(downloadUrl, hash, 0);
            request.Send();
        }

        public void Finish()
        {
            if (!request.isDone)
                throw new InvalidOperationException("Can't finish download operation that is in progress");
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogWarningFormat("Failed to download asset bundle from {0}", request.url);
                OnFailed();
            }
            else
            {
                OnLoaded(DownloadHandlerAssetBundle.GetContent(request));
            }
            request.Dispose();
        }

        public bool Update()
        {
            return !request.isDone;
        }

        protected abstract void OnLoaded(AssetBundle assetBundle);

        private void OnFailed()
        {
            BaseAssetLoaderService.OnAssetBundleLoadFailed();
        }
    }

    internal sealed class AssetBundleManifestDownloadOperation : BaseAssetBundleDownloadOperation
    {
        public AssetBundleManifestDownloadOperation(BaseAssetLoaderService baseAssetLoaderService, string downloadUrl) : base(
            baseAssetLoaderService,
            downloadUrl)
        {
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseAssetLoaderService.OnManifestAssetBundleLoaded(assetBundle);
        }
    }

    internal sealed class AssetBundleManifestLoadOperation : BaseAssetBundleLoadOperation
    {
        public AssetBundleManifestLoadOperation(BaseAssetLoaderService baseAssetLoaderService, string path) :
            base(baseAssetLoaderService, path)
        {
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseAssetLoaderService.OnManifestAssetBundleLoaded(assetBundle);
        }
    }

    internal sealed class AssetBundleDownloadOperation : BaseAssetBundleDownloadOperation
    {
        private readonly string assetBundleName;

        public AssetBundleDownloadOperation(BaseAssetLoaderService baseAssetLoaderService,
            string assetBundleName,
            string downloadUrl) : base(baseAssetLoaderService, downloadUrl)
        {
            this.assetBundleName = assetBundleName;
        }

        public AssetBundleDownloadOperation(BaseAssetLoaderService baseAssetLoaderService,
            string assetBundleName,
            string downloadUrl,
            Hash128 hash) : base(
            baseAssetLoaderService,
            downloadUrl,
            hash)
        {
            this.assetBundleName = assetBundleName;
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseAssetLoaderService.OnAssetBundleLoaded(assetBundleName, assetBundle);
        }
    }

    internal sealed class AssetBundleLoadOperation : BaseAssetBundleLoadOperation
    {
        private readonly string assetBundleName;

        public AssetBundleLoadOperation(BaseAssetLoaderService baseAssetLoaderService, string path) : base(baseAssetLoaderService,
            path)
        {
            assetBundleName = Path.GetFileNameWithoutExtension(path);
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseAssetLoaderService.OnAssetBundleLoaded(assetBundleName, assetBundle);
        }
    }

    internal abstract class BaseAssetLoadOperation : ILoadingOperation
    {
        protected readonly AssetBundleRequest Request;

        protected BaseAssetLoadOperation(AssetBundleRequest request)
        {
            Request = request;
        }

        public void Finish()
        {
            if (!Request.isDone)
                throw new InvalidOperationException("Can't finish loading operation that isn't done");
            OnFinish(Request);
        }

        public bool Update()
        {
            return !Request.isDone;
        }

        protected abstract void OnFinish(AssetBundleRequest request);
    }

    internal sealed class LoadAssetOperation<T> : BaseAssetLoadOperation where T : Object
    {
        private readonly Action<T> onLoaded;

        public LoadAssetOperation(AssetBundle assetBundle, string name, Action<T> onLoaded) : base(assetBundle
            .LoadAssetAsync(
                name))
        {
            this.onLoaded = onLoaded;
        }

        protected override void OnFinish(AssetBundleRequest request)
        {
            onLoaded(request.asset as T);
        }
    }

    internal sealed class LoadAllAssetsOperation<T> : BaseAssetLoadOperation where T : Object
    {
        private readonly Action<T[]> onLoaded;

        public LoadAllAssetsOperation(AssetBundle assetBundle, Action<T[]> onLoaded) : base(assetBundle
            .LoadAllAssetsAsync<T
            >())
        {
            this.onLoaded = onLoaded;
        }

        protected override void OnFinish(AssetBundleRequest request)
        {
            var loadedAssets = request.allAssets;
            var result = new T[loadedAssets.Length];
            for (var i = 0; i < loadedAssets.Length; i++)
                result[i] = loadedAssets[i] as T;
            onLoaded(result);
        }
    }

    public interface IAssetLoaderFactory
    {
        ILoadingOperation LoadAssetBundleManifest(string assetBundleName);
        ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash);
    }

    internal sealed class WebAssetLoaderFactory : IAssetLoaderFactory
    {
        private readonly BaseAssetLoaderService baseAssetLoaderService;
        private readonly string baseUrl;

        public WebAssetLoaderFactory(BaseAssetLoaderService baseAssetLoaderService, string baseUrl)
        {
            this.baseAssetLoaderService = baseAssetLoaderService;
            this.baseUrl = baseUrl;
        }

        public ILoadingOperation LoadAssetBundleManifest(string assetBundleName)
        {
            return new AssetBundleManifestDownloadOperation(baseAssetLoaderService, GetUrlForBundle(assetBundleName));
        }

        public ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash)
        {
            return new AssetBundleDownloadOperation(baseAssetLoaderService,
                assetBundleName,
                GetUrlForBundle(assetBundleName),
                hash);
        }

        private string GetUrlForBundle(string assetBundleName)
        {
            return string.Format("{0}/{1}", baseUrl, assetBundleName);
        }
    }

    internal sealed class LocalAssetLoaderFactory : IAssetLoaderFactory
    {
        private readonly BaseAssetLoaderService baseAssetLoaderService;
        private readonly string rootPath;

        public LocalAssetLoaderFactory(BaseAssetLoaderService baseAssetLoaderService, string rootPath)
        {
            this.baseAssetLoaderService = baseAssetLoaderService;
            this.rootPath = rootPath;
        }

        public ILoadingOperation LoadAssetBundleManifest(string assetBundleName)
        {
            return new AssetBundleManifestLoadOperation(baseAssetLoaderService, GetBundlePath(assetBundleName));
        }

        public ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash)
        {
            return new AssetBundleLoadOperation(baseAssetLoaderService, GetBundlePath(assetBundleName));
        }

        private string GetBundlePath(string assetBundleName)
        {
            return Path.Combine(rootPath, assetBundleName);
        }
    }

    internal sealed class PreloadAssetBundlesOperation : ILoadingOperation
    {
        private BaseAssetLoaderService assetLoaderService;
        private int bundlesToLoad = 0;
        private Action onLoaded;

        public PreloadAssetBundlesOperation(BaseAssetLoaderService assetLoaderService, string[] assetBundles, Action onLoaded)
        {
            bundlesToLoad = assetBundles.Length;
            this.onLoaded = onLoaded;
            foreach (var assetBundle in assetBundles)
            {
                assetLoaderService.LoadAssetBundle(assetBundle, bundle => bundlesToLoad--);
            }
        }

        public bool Update()
        {
            return bundlesToLoad > 0;
        }

        public void Finish()
        {
            if (onLoaded != null)
                onLoaded();
        }
    }
}