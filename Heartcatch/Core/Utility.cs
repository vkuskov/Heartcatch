using UnityEngine;

namespace Heartcatch.Core
{
    public static class Utility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";
        public const string AssetBundleSimulationMode = "__assetBundleSimulation";
        public const string GameConfigResource = "GameConfig";
        public const string RunnerResource = "Runner";

        public static string GetPlatformName()
        {
            return GetPlatformForAssetBundles(Application.platform);
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.tvOS:
                    return "tvOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    return "Linux";

                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
    }
}