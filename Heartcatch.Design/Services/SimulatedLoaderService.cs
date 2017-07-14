using System;
using System.Collections.Generic;
using Heartcatch.Design.Models;
using Heartcatch.Models;
using Heartcatch.Services;
using UnityEditor;
using UnityEngine;

namespace ShipGame.Core.Services
{
    public sealed class SimulatedLoaderService : ILoaderService
    {
        private readonly Dictionary<string, SimulatedAssetBundleModel> _assetBundles =
            new Dictionary<string, SimulatedAssetBundleModel>();

        public SimulatedLoaderService()
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(AssetBundleDescriptionModel)));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var description = AssetDatabase.LoadAssetAtPath<AssetBundleDescriptionModel>(path);
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

        public void Update()
        {
        }
    }
}
