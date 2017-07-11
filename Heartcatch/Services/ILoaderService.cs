using System;
using Heartcatch.Models;

namespace Heartcatch.Services
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