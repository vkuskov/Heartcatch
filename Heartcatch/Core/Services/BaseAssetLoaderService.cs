﻿using System;
using System.Collections.Generic;
using Heartcatch.Core.Models;
using UnityEngine;

namespace Heartcatch.Core.Services
{
    public abstract class BaseAssetLoaderService : IAssetLoaderService
    {
        private readonly Dictionary<string, AssetBundleModel> assetBundles = new Dictionary<string, AssetBundleModel>();


        private readonly Dictionary<string, AssetBundleModel> loadingAssetBundles =
            new Dictionary<string, AssetBundleModel>();

        private HashSet<string> preloadedBundles = new HashSet<string>();

        private readonly List<ILoadingOperation> loadingOperations = new List<ILoadingOperation>();
        private AssetBundleManifest assetBundleManifest;

        private IAssetLoaderFactory loaderFactory;

        public bool IsLoading
        {
            get { return loadingOperations.Count > 0; }
        }

        public bool IsInitialized
        {
            get { return assetBundleManifest != null; }
        }

        public void LoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded)
        {
            var bundle = GetAssetBundle(name);
            if (bundle.IsLoaded)
            {
                onLoaded(bundle);
            }
            else
            {
                if (!loadingAssetBundles.ContainsKey(name))
                    LoadAssetBundle(name);
                AddLoadingOperation(new WaitForAssetBundleToLoad(this, name, onLoaded));
            }
        }

        public void UnloadAll()
        {
            foreach (var it in assetBundles)
            {
                if (!preloadedBundles.Contains(it.Key))
                    it.Value.Unload();
            }
        }

        public void Preload(string[] assetBundles, Action onLoaded)
        {
            if (!IsInitialized)
                throw new LoadingException("Can't load bundles if loader wasn't initialized");
            if (assetBundles == null)
                throw new ArgumentNullException("assetBundles");
            foreach (var assetBundle in assetBundles)
            {
                MarkAssetBundleAsPreloaded(assetBundle);
            }
            AddLoadingOperation(new PreloadAssetBundlesOperation(this, assetBundles, onLoaded));
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

        [PostConstruct]
        public void Init()
        {
            loaderFactory = CreateAssetLoaderFactory();
            AddLoadingOperation(loaderFactory.LoadAssetBundleManifest(Utility.GetPlatformName()));
        }

        protected abstract IAssetLoaderFactory CreateAssetLoaderFactory();

        internal IAssetBundleModel GetLoadedAssetBundle(string name)
        {
            var bundle = GetAssetBundle(name);
            if (bundle.IsLoaded)
                return bundle;
            throw new LoadingException(string.Format("Can't get asset bundle \"{0}\" - it isn't loaded yet", name));
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

        internal void LoadAssetBundle(string name)
        {
            var bundle = GetAssetBundle(name);
            if (bundle.IsLoadedItself || loadingAssetBundles.ContainsKey(name))
                return;
            Debug.LogFormat("Loading asset bundle: {0}", name);
            loadingAssetBundles.Add(name, bundle);
            AddLoadingOperation(loaderFactory.LoadAssetBundle(name, assetBundleManifest.GetAssetBundleHash(name)));
        }

        private void MarkAssetBundleAsPreloaded(string assetBundle)
        {
            if (preloadedBundles.Contains(assetBundle))
            {
                return;
            }
            preloadedBundles.Add(assetBundle);
            var dependencies = assetBundleManifest.GetDirectDependencies(assetBundle);
            foreach (var dependency in dependencies)
            {
                MarkAssetBundleAsPreloaded(dependency);
            }
        }
    }
}