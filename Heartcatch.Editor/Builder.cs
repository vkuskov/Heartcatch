using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Heartcatch.Models;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Editor
{
    public class Builder
    {
        public const string BUILD = "Build";

        [MenuItem("Assets/AssetBundles/Build Bundles")]
        public static void BuildBundlesForCurrentPlatform()
        {
            BuildBundles(EditorUserBuildSettings.activeBuildTarget, false);
        }

        [MenuItem("Assets/AssetBundles/Build Dev")]
        public static void BuildGameForCurrentPlatform()
        {
            BuildGame(EditorUserBuildSettings.activeBuildTarget, true, false);
        }

        public static void BuildGameForCurrentPlatformDev()
        {
            BuildGame(EditorUserBuildSettings.activeBuildTarget, false, false);
        }

        [MenuItem("Assets/AssetBundles/Build Release")]
        public static void BuildReleaseForCurrentPlatform()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                "RELEASE_BUILD");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                "LOCAL_BUNDLES");
            BuildGame(EditorUserBuildSettings.activeBuildTarget, true, true);
        }

        public static void BuildCIForCurrentPlatform()
        {
            BuildCI(EditorUserBuildSettings.activeBuildTarget);
        }

        public static void BuildCI(BuildTarget target)
        {
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                target = target,
                locationPathName = Path.Combine(BUILD, getBuildTargetName(target)),
                scenes = getScenesToBuild(),
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
                {
                    copyAssetBundlesToStreamingAssets(assetBundlePath, localBuild);
                }
            }
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                target = target,
                assetBundleManifestPath = Path.Combine(assetBundlePath, Utility.GetPlatformName()),
                locationPathName = Path.Combine(BUILD, getBuildTargetName(target)),
                scenes = getScenesToBuild(),
                options = BuildOptions.StrictMode
            };
            BuildPipeline.BuildPlayer(options);
        }

        private static string[] getScenesToBuild()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        public static string getBuildTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "ShipGame.apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "ShipGame/ShipGame.exe";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "ShipGame.app";
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneLinux64:
                    return "ShipGame";
                case BuildTarget.WebGL:
                case BuildTarget.iOS:
                    return "";
                // Add more build targets for your own.
                default:
                    throw new ArgumentException("Target is not implemented");
            }
        }

        private static void copyAssetBundlesToStreamingAssets(string path, bool fullCopy)
        {
            if (Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.Delete(Application.streamingAssetsPath, true);
            }
            var fullPath = Path.Combine(Application.streamingAssetsPath, path);
            Directory.CreateDirectory(fullPath);
            foreach (var it in getAssetsForStreamingAssets(fullCopy))
            {
                copySingleBundle(path, fullPath, it);
            }
            copySingleBundle(path, fullPath, Utility.GetPlatformName());
        }

        private static void copySingleBundle(string source, string destination, string name)
        {
            var srcPath = Path.Combine(source, name);
            var destPath = Path.Combine(destination, name);
            File.Copy(srcPath, destPath);
        }

        private static IEnumerable<string> getAssetsForStreamingAssets(bool fullCopy)
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(AssetBundleDescriptionModel)));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var desc = AssetDatabase.LoadAssetAtPath<AssetBundleDescriptionModel>(path);
                if (fullCopy || desc.IncludeToStreamingAssets)
                {
                    yield return desc.Name;
                }
            }
        }

        public static AssetBundleManifest BuildBundles(BuildTarget target, bool preferLZ4)
        {
            Debug.Log("Build asset bundles");
            var path = GetAssetBundlePath(target);
            Directory.CreateDirectory(path);
            var bundles = getAssetBundlesToBuild().ToArray();
            var options = BuildAssetBundleOptions.StrictMode;
            if (target == BuildTarget.WebGL)
            {
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }
            else if (preferLZ4)
            {
                options |= BuildAssetBundleOptions.ChunkBasedCompression;
            }
            return BuildPipeline.BuildAssetBundles(path,
                bundles,
                options,
                target);
        }

        private static IEnumerable<AssetBundleBuild> getAssetBundlesToBuild()
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(AssetBundleDescriptionModel)));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var desc = AssetDatabase.LoadAssetAtPath<AssetBundleDescriptionModel>(path);
                var allAssets = desc.GetAssetPaths().ToArray();
                var bundle = new AssetBundleBuild();
                Debug.LogFormat("Bundle: {0}", desc.Name);
                bundle.assetBundleName = desc.Name;
                bundle.assetNames = allAssets;
                yield return bundle;
            }
        }

        public static string GetAssetBundlePathForCurrentPlatform()
        {
            return GetAssetBundlePath(EditorUserBuildSettings.activeBuildTarget);
        }

        public static string GetAssetBundlePath(BuildTarget target)
        {
            var platform = Utility.GetPlatformForAssetBundles(target);
            return Path.Combine(Utility.ASSET_BUNDLES_OUTPUT_PATH, platform);
        }

        const string SIMULATION_MODE_MENU = "Assets/AssetBundles/Simulation Mode";

        [MenuItem(SIMULATION_MODE_MENU)]
        public static void ToggleSimulationMode()
        {
            var simulationMode = PlayerPrefs.GetInt(Utility.ASSET_BUNDLE_SIMULATION_MODE, 0) != 0;
            simulationMode = !simulationMode;
            var set = simulationMode ? 1 : 0;
            PlayerPrefs.SetInt(Utility.ASSET_BUNDLE_SIMULATION_MODE, set);
        }

        [MenuItem(SIMULATION_MODE_MENU, true)]
        public static bool ToggleSimulationModeValidate()
        {
            var simulationMode = PlayerPrefs.GetInt(Utility.ASSET_BUNDLE_SIMULATION_MODE, 0) != 0;
            Menu.SetChecked(SIMULATION_MODE_MENU, simulationMode);
            return !Application.isPlaying;
        }
    }
}