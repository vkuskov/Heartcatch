using System.Collections.Generic;

namespace Heartcatch.Design.Models
{
    public interface IAssetBundleDescriptionModel
    {
        string Name { get; }
        bool IncludeToStreamingAssets { get; }
        IEnumerable<AssetPath> GetAssetPaths();
    }
}