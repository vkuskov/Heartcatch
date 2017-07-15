using UnityEngine;

namespace Heartcatch.Core.Models
{
    public interface IResourceRequestModel
    {
        void RequestResource(string name, AssetReference assetReference);
        T GetResource<T>(string name) where T : Object;
    }
}