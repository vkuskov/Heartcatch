using System;
using System.Collections.Generic;
using System.IO;
using Heartcatch.Core.Models;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heartcatch.Design.Models
{
    public sealed class SimulatedAssetBundleModel : IAssetBundleModel
    {
        private readonly List<string> allScenes = new List<string>();
        private readonly string name;
        private readonly Dictionary<string, string> nameToPath = new Dictionary<string, string>();

        public SimulatedAssetBundleModel(IAssetBundleDescriptionModel description)
        {
            name = description.Name;
            foreach (var path in description.GetAssetPaths())
            {
                if (Path.GetExtension(path.HiDefAssetPath) == ".unity")
                    allScenes.Add(path.HiDefAssetPath);
                nameToPath.Add(path.Name, path.HiDefAssetPath);
            }
        }

        public void LoadAsset<T>(string name, Action<T> onLoaded) where T : Object
        {
            string path;
            if (nameToPath.TryGetValue(name, out path))
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                onLoaded(asset);
            }
            else
            {
                Debug.LogErrorFormat("Can't load asset {0} from asset bundle {1}", name, this.name);
            }
        }

        public void LoadAllAssets<T>(Action<T[]> onLoaded) where T : Object
        {
            var result = new List<T>();
            foreach (var it in nameToPath)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(it.Value);
                if (asset != null)
                    result.Add(asset);
            }
            onLoaded(result.ToArray());
        }

        public string GetScenePath(int index)
        {
            return allScenes[index];
        }

        public void Unload()
        {
        }
    }
}