using System;
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
        private readonly BaseLoaderService baseLoaderService;
        private readonly string name;
        private readonly Action<IAssetBundleModel> onLoaded;

        public WaitForAssetBundleToLoad(BaseLoaderService baseLoaderService, string name, Action<IAssetBundleModel> onLoaded)
        {
            this.baseLoaderService = baseLoaderService;
            this.name = name;
            this.onLoaded = onLoaded;
        }

        public bool Update()
        {
            return !baseLoaderService.IsAssetBundleLoaded(name);
        }

        public void Finish()
        {
            onLoaded(baseLoaderService.GetLoadedAssetBundle(name));
        }
    }

    internal abstract class BaseAssetBundleLoadOperation : ILoadingOperation
    {
        private readonly AssetBundleCreateRequest createRequest;
        protected readonly BaseLoaderService BaseLoaderService;

        protected BaseAssetBundleLoadOperation(BaseLoaderService baseLoaderService, string path)
        {
            BaseLoaderService = baseLoaderService;
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
        protected readonly BaseLoaderService BaseLoaderService;
        private readonly UnityWebRequest request;

        protected BaseAssetBundleDownloadOperation(BaseLoaderService baseLoaderService, string downloadUrl)
        {
            BaseLoaderService = baseLoaderService;
            request = UnityWebRequest.GetAssetBundle(downloadUrl);
            request.Send();
        }

        protected BaseAssetBundleDownloadOperation(BaseLoaderService baseLoaderService, string downloadUrl, Hash128 hash)
        {
            BaseLoaderService = baseLoaderService;
            request = UnityWebRequest.GetAssetBundle(downloadUrl, hash, 0);
            request.Send();
        }

        public void Finish()
        {
            if (!request.isDone)
                throw new InvalidOperationException("Can't finish download operation that is in progress");
            if (request.isNetworkError)
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
            BaseLoaderService.OnAssetBundleLoadFailed();
        }
    }

    internal sealed class AssetBundleManifestDownloadOperation : BaseAssetBundleDownloadOperation
    {
        public AssetBundleManifestDownloadOperation(BaseLoaderService baseLoaderService, string downloadUrl) : base(
            baseLoaderService,
            downloadUrl)
        {
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseLoaderService.OnManifestAssetBundleLoaded(assetBundle);
        }
    }

    internal sealed class AssetBundleManifestLoadOperation : BaseAssetBundleLoadOperation
    {
        public AssetBundleManifestLoadOperation(BaseLoaderService baseLoaderService, string path) :
            base(baseLoaderService, path)
        {
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseLoaderService.OnManifestAssetBundleLoaded(assetBundle);
        }
    }

    internal sealed class AssetBundleDownloadOperation : BaseAssetBundleDownloadOperation
    {
        private readonly string assetBundleName;

        public AssetBundleDownloadOperation(BaseLoaderService baseLoaderService,
            string assetBundleName,
            string downloadUrl) : base(baseLoaderService, downloadUrl)
        {
            this.assetBundleName = assetBundleName;
        }

        public AssetBundleDownloadOperation(BaseLoaderService baseLoaderService,
            string assetBundleName,
            string downloadUrl,
            Hash128 hash) : base(
            baseLoaderService,
            downloadUrl,
            hash)
        {
            this.assetBundleName = assetBundleName;
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseLoaderService.OnAssetBundleLoaded(assetBundleName, assetBundle);
        }
    }

    internal sealed class AssetBundleLoadOperation : BaseAssetBundleLoadOperation
    {
        private readonly string assetBundleName;

        public AssetBundleLoadOperation(BaseLoaderService baseLoaderService, string path) : base(baseLoaderService, path)
        {
            assetBundleName = Path.GetFileNameWithoutExtension(path);
        }

        protected override void OnLoaded(AssetBundle assetBundle)
        {
            BaseLoaderService.OnAssetBundleLoaded(assetBundleName, assetBundle);
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
        private readonly string baseUrl;
        private readonly BaseLoaderService baseLoaderService;

        public WebAssetLoaderFactory(BaseLoaderService baseLoaderService, string baseUrl)
        {
            this.baseLoaderService = baseLoaderService;
            this.baseUrl = baseUrl;
        }

        public ILoadingOperation LoadAssetBundleManifest(string assetBundleName)
        {
            return new AssetBundleManifestDownloadOperation(baseLoaderService, GetUrlForBundle(assetBundleName));
        }

        public ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash)
        {
            return new AssetBundleDownloadOperation(baseLoaderService,
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
        private readonly BaseLoaderService baseLoaderService;
        private readonly string rootPath;

        public LocalAssetLoaderFactory(BaseLoaderService baseLoaderService, string rootPath)
        {
            this.baseLoaderService = baseLoaderService;
            this.rootPath = rootPath;
        }

        public ILoadingOperation LoadAssetBundleManifest(string assetBundleName)
        {
            return new AssetBundleManifestLoadOperation(baseLoaderService, GetBundlePath(assetBundleName));
        }

        public ILoadingOperation LoadAssetBundle(string assetBundleName, Hash128 hash)
        {
            return new AssetBundleLoadOperation(baseLoaderService, GetBundlePath(assetBundleName));
        }

        private string GetBundlePath(string assetBundleName)
        {
            return Path.Combine(rootPath, assetBundleName);
        }
    }
}