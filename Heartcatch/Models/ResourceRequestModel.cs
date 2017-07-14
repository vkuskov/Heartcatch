using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using strange.extensions.pool.api;
using Object = UnityEngine.Object;

namespace Heartcatch.Models
{
    public sealed class ResourceRequestModel : IResourceRequestModel
    {
        private Dictionary<string, AssetReference> requestedResources = new Dictionary<string, AssetReference>();
        private Dictionary<string, Object> loadedResources = new Dictionary<string, Object>();

        public void RequestResource(string name, AssetReference assetReference)
        {
            if (!requestedResources.ContainsKey(name))
            {
                requestedResources.Add(name, assetReference);
            }
            else
            {
                throw new Exception(string.Format("Resource {0} is already requested", name));
            }
        }

        public T GetResource<T>(string name) where T : Object
        {
            Object result;
            if (loadedResources.TryGetValue(name, out result))
            {
                var realResult = result as T;
                if (realResult != null)
                {
                    return realResult;
                }
                throw new Exception(string.Format("Resource {0} excepted to be {1} but it's {2}", name, typeof(T), result.GetType()));
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
