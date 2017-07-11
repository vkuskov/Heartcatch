using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Heartcatch.Models
{
    [CreateAssetMenu(menuName = "XShooter/Asset Bundle")]
    public sealed class AssetBundleDescriptionModel : ScriptableObject, IAssetBundleDescriptionModel
    {
        [SerializeField] private List<Object> _assets;

        [SerializeField] private bool _includeToStreamingAssets;

        [SerializeField] private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IncludeToStreamingAssets
        {
            get { return _includeToStreamingAssets; }
            set { _includeToStreamingAssets = value; }
        }

        public IEnumerable<string> GetAssetPaths()
        {
#if UNITY_EDITOR
            if (_assets != null)
                foreach (var asset in _assets)
                    yield return UnityEditor.AssetDatabase.GetAssetPath(asset);
#else
            return null;
#endif
        }
    }
}