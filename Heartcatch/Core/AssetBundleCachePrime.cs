using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Heartcatch.Core
{
    public sealed class AssetBundleCachePrime : MonoBehaviour
    {
        private const string StartupScene = "StartUp";

        public IEnumerator Start()
        {
            var manifest = LoadBundle(Utility.GetPlatformName());
            yield return manifest.Send();
            if (!manifest.isError)
            {
                var manifestBundle = DownloadHandlerAssetBundle.GetContent(manifest);
                var assetBundleManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                foreach (var bundle in assetBundleManifest.GetAllAssetBundles())
                {
                    var loadBundle = LoadBundle(bundle, assetBundleManifest.GetAssetBundleHash(bundle));
                    yield return loadBundle.Send();
                    if (!loadBundle.isError)
                    {
                        var assetBundle = DownloadHandlerAssetBundle.GetContent(loadBundle);
                        assetBundle.Unload(true);
                    }
                    loadBundle.Dispose();
                }
                manifestBundle.Unload(true);
            }
            manifest.Dispose();
            Resources.UnloadUnusedAssets();
            GC.Collect();
            PlayerPrefs.SetInt(Runner.CachePrimedFlag, 1);
            SceneManager.LoadScene(StartupScene);
            Runner.StartUp();
        }

        private UnityWebRequest LoadBundle(string name)
        {
            var url = getURL(name);
            var request = UnityWebRequest.GetAssetBundle(url);
            return request;
        }

        private UnityWebRequest LoadBundle(string name, Hash128 hash)
        {
            var url = getURL(name);
            var request = UnityWebRequest.GetAssetBundle(url, hash, 0);
            return request;
        }

        private string getURL(string name)
        {
            return string.Format("file://{0}/{1}/{2}/{3}",
                                 Application.streamingAssetsPath,
                                 Utility.AssetBundlesOutputPath,
                                 Utility.GetPlatformName(),
                                 name);
        }
    }
}
