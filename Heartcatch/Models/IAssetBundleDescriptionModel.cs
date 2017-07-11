using System.Collections.Generic;

namespace Heartcatch.Models
{
    public interface IAssetBundleDescriptionModel
    {
        string Name { get; }
        bool IncludeToStreamingAssets { get; }
        IEnumerable<string> GetAssetPaths();
    }
}