using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Design.Models
{
    [CreateAssetMenu(menuName = "Heartcatch/Asset Bundle")]
    public sealed class AssetBundleDescriptionModel : ScriptableObject, IAssetBundleDescriptionModel
    {
        [SerializeField] private string name;
        [SerializeField] private List<Asset> assets;

        [SerializeField] private bool includeToStreamingAssets;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Asset> Assets
        {
            get { return assets; }
            set { assets = value; }
        }

        public bool IncludeToStreamingAssets
        {
            get { return includeToStreamingAssets; }
            set { includeToStreamingAssets = value; }
        }

        public IEnumerable<AssetPath> GetAssetPaths()
        {
            if (assets != null)
                foreach (var asset in assets)
                {
                    var hiDefAssetPath = AssetDatabase.GetAssetPath(asset.HiDefAsset);
                    yield return new AssetPath {Name = asset.Name, HiDefAssetPath = hiDefAssetPath};
                }
        }
    }
}