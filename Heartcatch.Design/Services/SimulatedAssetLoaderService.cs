using System;
using System.Collections.Generic;
using Heartcatch.Core.Models;
using Heartcatch.Core.Services;
using Heartcatch.Design.Models;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Design.Services
{
    public sealed class SimulatedAssetLoaderService : IAssetLoaderService
    {
        private readonly Dictionary<string, SimulatedAssetBundleModel> _assetBundles =
            new Dictionary<string, SimulatedAssetBundleModel>();

        public SimulatedAssetLoaderService()
        {
            LoadAssetBundles<AssetBundleDescriptionModel>();
            LoadAssetBundles<UIAssetBundleDescriptionModel>();
        }

        private void LoadAssetBundles<T>()
            where T : ScriptableObject, IAssetBundleDescriptionModel
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var description = AssetDatabase.LoadAssetAtPath<T>(path);
                _assetBundles.Add(description.Name, new SimulatedAssetBundleModel(description));
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
            if (_assetBundles.TryGetValue(name, out assetBundle))
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