using System;
using Heartcatch.Core.Models;

namespace Heartcatch.Core.Services
{
    public interface IAssetLoaderService
    {
        bool IsLoading { get; }
        bool IsInitialized { get; }
        void LoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded);
        void UnloadAll();
        void Preload(string[] assetBundles, Action onLoaded);
        void Update();
    }
}