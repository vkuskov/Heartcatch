using System;

namespace Heartcatch.Models
{
    [Serializable]
    public struct AssetReference
    {
        public string AssetBundle;
        public string AssetName;

        public AssetReference(string assetBundle, string assetName)
        {
            this.AssetBundle = assetBundle;
            this.AssetName = assetName;
        }
    }
}