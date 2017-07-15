using System.IO;
using UnityEngine;

namespace Heartcatch.Core.Services
{
    public sealed class LocalLoaderService : BaseLoaderService
    {
        protected override IAssetLoaderFactory CreateAssetLoaderFactory()
        {
            var path = Path.Combine(Application.streamingAssetsPath,
                Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformName()));
            return new LocalAssetLoaderFactory(this, path);
        }
    }
}
