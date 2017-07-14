using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Heartcatch.Services
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
