using System;
using Heartcatch.Core.Models;

namespace Heartcatch.Core.Services
{
    public interface ILoaderService
    {
        bool IsLoading { get; }
        bool IsInitialized { get; }
        void LoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded);
        void GetOrLoadAssetBundle(string name, Action<IAssetBundleModel> onLoaded);
        void UnloadAll();
        void Update();
    }
}