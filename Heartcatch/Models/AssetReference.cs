using System;

namespace Heartcatch.Models
{
    [Serializable]
    public struct AssetReference
    {
        public string assetBundle;
        public string assetName;

        public AssetReference(string assetBundle, string assetName)
        {
            this.assetBundle = assetBundle;
            this.assetName = assetName;
        }
    }
}