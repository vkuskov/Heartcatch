
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipGame.Core.Models
{
    public sealed class SimulatedAssetBundleModel : IAssetBundleModel
    {
        private readonly List<string> _allScenes = new List<string>();
        private readonly string _name;
        private readonly Dictionary<string, string> _nameToPath = new Dictionary<string, string>();

        public SimulatedAssetBundleModel(IAssetBundleDescriptionModel description)
        {
            _name = description.Name;
            foreach (var path in description.GetAssetPaths())
            {
                if (Path.GetExtension(path) == ".unity")
                    _allScenes.Add(path);
                _nameToPath.Add(Path.GetFileNameWithoutExtension(path), path);
            }
        }

        public void LoadAsset<T>(string name, Action<T> onLoaded) where T : Object
        {
            string path;
            if (_nameToPath.TryGetValue(name, out path))
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                onLoaded(asset);
            }
            else
            {
                Debug.LogErrorFormat("Can't load asset {0} from asset bundle {1}", name, _name);
            }
        }

        public void LoadAllAssets<T>(Action<T[]> onLoaded) where T : Object
        {
            var result = new List<T>();
            foreach (var it in _nameToPath)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(it.Value);
                if (asset != null)
                    result.Add(asset);
            }
            onLoaded(result.ToArray());
        }

        public string GetScenePath(int index)
        {
            return _allScenes[index];
        }

        public void Unload()
        {
        }
    }
}
#endif // UNITY_EDITOR