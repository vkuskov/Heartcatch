using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Heartcatch.Core.Models;
using Heartcatch.Design.Models;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Editor
{
    public class Builder
    {
        public const string Build = "Build";

        private const string SimulationModeMenu = "Heartcatch/Simulation Mode";

        [MenuItem("Heartcatch/Build Remote Bundles", priority = 100)]
        public static void BuildBundlesForCurrentPlatform()
        {
            Debug.Log("Buidling remote bundles...");
            BuildBundles(EditorUserBuildSettings.activeBuildTarget, false);
            Debug.Log("DONE!");
        }

        [MenuItem("Heartcatch/Build Local Bundles", priority = 101)]
        public static void BuildLocalBundlesForCurrentPlatform()
        {
            Debug.Log("Building local bundles...");
            BuildBundles(EditorUserBuildSettings.activeBuildTarget, true);
            Debug.Log("Copying bundles to StreamingAssets...");
            CopyAssetBundlesToStreamingAssets(GetAssetBundlePath(EditorUserBuildSettings.activeBuildTarget), true);
            Debug.Log("DONE!");
        }

        [MenuItem("Heartcatch/Build Dev (Remote bundles)", priority = 200)]
        public static void BuildGameForCurrentPlatform()
        {
            BuildGame(EditorUserBuildSettings.activeBuildTarget, true, false);
        }

        public static void BuildGameForCurrentPlatformDev()
        {
            BuildGame(EditorUserBuildSettings.activeBuildTarget, false, false);
        }

        [MenuItem("Heartcatch/Build Release", priority = 201)]
        public static void BuildReleaseForCurrentPlatform()
        {
            BuildGame(EditorUserBuildSettings.activeBuildTarget, true, true);
        }

        public static void BuildCiForCurrentPlatform()
        {
            BuildCi(EditorUserBuildSettings.activeBuildTarget);
        }

        public static void BuildCi(BuildTarget target)
        {
            var options = new BuildPlayerOptions
            {
                target = target,
                locationPathName = Path.Combine(Build, GetBuildTargetName(target)),
                scenes = GetScenesToBuild(),
                options = BuildOptions.StrictMode | BuildOptions.BuildScriptsOnly
            };
            BuildPipeline.BuildPlayer(options);
        }

        public static void BuildGame(BuildTarget target, bool includeAssetBundles, bool localBuild)
        {
            var assetBundlePath = GetAssetBundlePath(target);
            if (includeAssetBundles)
            {
                var manifest = BuildBundles(target, localBuild);
                if (manifest != null)
                    CopyAssetBundlesToStreamingAssets(assetBundlePath, localBuild);
            }
            var options = new BuildPlayerOptions
            {
                target = target,
                assetBundleManifestPath = Path.Combine(assetBundlePath, Utility.GetPlatformName()),
                locationPathName = Path.Combine(Build, GetBuildTargetName(target)),
                scenes = GetScenesToBuild(),
                options = BuildOptions.StrictMode
            };
            Debug.Log("Buidling player...");
            BuildPipeline.BuildPlayer(options);
            Debug.Log("DONE!");
        }

        private static string[] GetScenesToBuild()
        {
            var levels = new List<string>();
            for (var i = 0; i < EditorBuildSettings.scenes.Length; ++i)
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);

            return levels.ToArray();
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            var config = Resources.Load<GameConfigModel>(Core.Utility.GameConfigResource);
            if (config == null)
                throw new BuildException(string.Format("Can't load config from resource {0}",
                    Core.Utility.GameConfigResource));
            switch (target)
            {
                case BuildTarget.Android:
                    return string.Format("{0}.apk", config.GameName);
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return string.Format("{0}/Game.exe", config.GameName);
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return string.Format("{0}.app", config.GameName);
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneLinux64:
                    return config.GameName;
                case BuildTarget.WebGL:
                case BuildTarget.iOS:
                    return "";
                // Add more build targets for your own.
                default:
                    throw new ArgumentException("Target is not implemented");
            }
        }

        private static void CopyAssetBundlesToStreamingAssets(string sourcePath, bool fullCopy)
        {
            if (Directory.Exists(Application.streamingAssetsPath))
                Directory.Delete(Application.streamingAssetsPath, true);
            var fullPath = Path.Combine(Application.streamingAssetsPath, sourcePath);
            Directory.CreateDirectory(fullPath);
            foreach (var it in GetAssetsForStreamingAssets<AssetBundleDescriptionModel>(fullCopy))
                CopySingleBundle(sourcePath, fullPath, it);
            foreach (var it in GetAssetsForStreamingAssets<UiAssetBundleDescriptionModel>(fullCopy))
            {
                CopySingleBundle(sourcePath, fullPath, it);
            }
            CopySingleBundle(sourcePath, fullPath, Utility.GetPlatformName());
        }

        private static void CopySingleBundle(string source, string destination, string name)
        {
            var srcPath = Path.Combine(source, name);
            var destPath = Path.Combine(destination, name);
            File.Copy(srcPath, destPath);
        }

        private static IEnumerable<string> GetAssetsForStreamingAssets<T>(bool fullCopy) where T : ScriptableObject, IAssetBundleDescriptionModel
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var desc = AssetDatabase.LoadAssetAtPath<T>(path);
                if (fullCopy || desc.IncludeToStreamingAssets)
                    yield return desc.Name;
            }
        }

        public static AssetBundleManifest BuildBundles(BuildTarget target, bool preferLz4)
        {
            var path = GetAssetBundlePath(target);
            Directory.CreateDirectory(path);
            var bundles = GetAssetBundlesToBuild().ToArray();
            var options = BuildAssetBundleOptions.StrictMode;
            if (target == BuildTarget.WebGL)
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
            else if (preferLz4)
                options |= BuildAssetBundleOptions.ChunkBasedCompression;
            return BuildPipeline.BuildAssetBundles(path,
                bundles,
                options,
                target);
        }

        private static IEnumerable<AssetBundleBuild> GetAssetBundlesToBuild()
        {
            var allBundles = new List<AssetBundleBuild>();
            allBundles.AddRange(GetAssetBundles<AssetBundleDescriptionModel>());
            allBundles.AddRange(GetAssetBundles<UiAssetBundleDescriptionModel>());
            return allBundles;
        }

        private static IEnumerable<AssetBundleBuild> GetAssetBundles<T>()
            where T : ScriptableObject, IAssetBundleDescriptionModel
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var desc = AssetDatabase.LoadAssetAtPath<T>(path);
                var allAssets = desc.GetAssetPaths().ToArray();
                var bundle = new AssetBundleBuild();
                bundle.assetBundleName = desc.Name;
                bundle.assetNames = GetAllAssetPaths(allAssets);
                bundle.addressableNames = GetAllAddressableNames(allAssets);
                yield return bundle;
            }
        }

        private static string[] GetAllAddressableNames(AssetPath[] paths)
        {
            var result = new string[paths.Length];
            for (var i = 0; i < paths.Length; ++i)
                result[i] = paths[i].Name;
            return result;
        }

        private static string[] GetAllAssetPaths(AssetPath[] paths)
        {
            var result = new string[paths.Length];
            for (var i = 0; i < paths.Length; ++i)
                result[i] = paths[i].HiDefAssetPath;
            return result;
        }

        public static string GetAssetBundlePathForCurrentPlatform()
        {
            return GetAssetBundlePath(EditorUserBuildSettings.activeBuildTarget);
        }

        public static string GetAssetBundlePath(BuildTarget target)
        {
            var platform = Utility.GetPlatformForAssetBundles(target);
            return Path.Combine(Core.Utility.AssetBundlesOutputPath, platform);
        }

        [MenuItem(SimulationModeMenu, priority = 10000)]
        public static void ToggleSimulationMode()
        {
            var simulationMode = PlayerPrefs.GetInt(Core.Utility.AssetBundleSimulationMode, 0) != 0;
            simulationMode = !simulationMode;
            var set = simulationMode ? 1 : 0;
            PlayerPrefs.SetInt(Core.Utility.AssetBundleSimulationMode, set);
        }

        [MenuItem(SimulationModeMenu, true)]
        public static bool ToggleSimulationModeValidate()
        {
            var simulationMode = PlayerPrefs.GetInt(Core.Utility.AssetBundleSimulationMode, 0) != 0;
            Menu.SetChecked(SimulationModeMenu, simulationMode);
            return !Application.isPlaying;
        }
    }

    public class BuildException : Exception
    {
        public BuildException(string message) : base(message)
        {
        }
    }
}