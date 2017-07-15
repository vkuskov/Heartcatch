using System;
using Object = UnityEngine.Object;

namespace Heartcatch.Core.Models
{
    public interface IAssetBundleModel
    {
        void LoadAsset<T>(string name, Action<T> onLoaded) where T : Object;
        void LoadAllAssets<T>(Action<T[]> onLoaded) where T : Object;
        string GetScenePath(int index);
        void Unload();
    }
}