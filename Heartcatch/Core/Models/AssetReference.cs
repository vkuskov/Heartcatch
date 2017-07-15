using System;

namespace Heartcatch.Core.Models
{
    [Serializable]
    public struct AssetReference
    {
        public string AssetBundle;
        public string AssetName;

        public AssetReference(string assetBundle, string assetName)
        {
            AssetBundle = assetBundle;
            AssetName = assetName;
        }
    }
}