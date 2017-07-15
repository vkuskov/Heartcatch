using System.Collections.Generic;
using UnityEngine;

namespace Heartcatch.Core.Models
{
    public sealed class ResourceRequestModel : IResourceRequestModel
    {
        private readonly Dictionary<string, Object> loadedResources = new Dictionary<string, Object>();

        private readonly Dictionary<string, AssetReference> requestedResources =
            new Dictionary<string, AssetReference>();

        public void RequestResource(string name, AssetReference assetReference)
        {
            if (!requestedResources.ContainsKey(name))
                requestedResources.Add(name, assetReference);
            else
                throw new Exception(string.Format("Resource {0} is already requested", name));
        }

        public T GetResource<T>(string name) where T : Object
        {
            Object result;
            if (loadedResources.TryGetValue(name, out result))
            {
                var realResult = result as T;
                if (realResult != null)
                    return realResult;
                throw new Exception(string.Format("Resource {0} excepted to be {1} but it's {2}", name, typeof(T),
                    result.GetType()));
            }
            throw new Exception(string.Format("Resource {0} wasn't loaded", name));
        }

        public void OnResourceLoaded(string name, Object resource)
        {
            loadedResources.Add(name, resource);
        }

        public bool IsAllResourcesLoaded()
        {
            return loadedResources.Count == requestedResources.Count;
        }

        public IEnumerator<KeyValuePair<string, AssetReference>> GetEnumerator()
        {
            return requestedResources.GetEnumerator();
        }

        public void Release()
        {
            requestedResources.Clear();
            loadedResources.Clear();
        }
    }
}