using System;
using System.Collections.Generic;
using System.IO;
using Heartcatch.Core.Models;
using Heartcatch.Core.Services;
using Heartcatch.Design.Models;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Design.Services
{
    public sealed class SimulatedAssetLoaderService : IAssetLoaderService
    {
        private readonly Dictionary<string, SimulatedAssetBundleModel> assetBundles =
            new Dictionary<string, SimulatedAssetBundleModel>();

        public SimulatedAssetLoaderService()
        {
            var allAssetBundles = AssetDatabase.GetAllAssetBundleNames();
            foreach (var assetBundle in allAssetBundles)
            {
                assetBundles.Add(assetBundle, new SimulatedAssetBundleModel(assetBundle));
            }
        }

        public bool IsLoading
        {
            get { return false; }
        }

        public bool IsInitialized
        {
            get { return true; }
        }

        public void LoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded)
        {
            SimulatedAssetBundleModel assetBundle;
            if (assetBundles.TryGetValue(name, out assetBundle))
                onLoaded(assetBundle);
            else
                Debug.LogErrorFormat("Can't load asset bundle {0}", name);
        }

        public void GetOrLoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded)
        {
            LoadAssetBundle(name, onLoaded);
        }

        public void UnloadAll()
        {
        }

        public void Preload(string[] assetBundles, Action onLoaded)
        {
            onLoaded();
        }

        public void Update()
        {
        }
    }
}