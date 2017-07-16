using System.Collections.Generic;
using Heartcatch.Design.Models;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Editor
{
    [CustomEditor(typeof(AssetBundleDescriptionModel))]
    public sealed class AssetBundleEditor : UnityEditor.Editor
    {
        private string newAssetName;
        private Object newHiDefAsset;

        public override void OnInspectorGUI()
        {
            var assetBundle = (AssetBundleDescriptionModel) target;
            var dirty = false;
            EditorGUI.BeginChangeCheck();
            assetBundle.Name = EditorGUILayout.TextField("Bundle:", assetBundle.Name);
            dirty |= EditorGUI.EndChangeCheck();
            EditorGUILayout.Space();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.Space();
            newAssetName = EditorGUILayout.TextField("Name:", newAssetName);
            newHiDefAsset = EditorGUILayout.ObjectField(newHiDefAsset, typeof(Object), false);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(newAssetName) || newHiDefAsset == null);
            if (GUILayout.Button("Add"))
            {
                if (assetBundle.Assets == null)
                {
                    assetBundle.Assets = new List<Asset>();
                }
                assetBundle.Assets.Add(new Asset {Name = newAssetName, HiDefAsset = newHiDefAsset});
                EditorUtility.SetDirty(assetBundle);
                newAssetName = null;
                newHiDefAsset = null;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.Space();
            if (assetBundle.Assets != null)
            {
                for (var i = 0; i < assetBundle.Assets.Count;)
                {
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                    var asset = assetBundle.Assets[i];
                    EditorGUILayout.BeginHorizontal();
                    var toRemove = GUILayout.Button("X");
                    EditorGUI.BeginChangeCheck();
                    asset.Name = EditorGUILayout.TextField("", asset.Name);
                    EditorGUILayout.EndHorizontal();
                    asset.HiDefAsset = EditorGUILayout.ObjectField(asset.HiDefAsset, typeof(Object), false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        assetBundle.Assets[i] = asset;
                        dirty = true;
                    }
                    if (toRemove)
                        assetBundle.Assets.RemoveAt(i);
                    else
                        i++;
                }
            }
            if (dirty)
            {
                EditorUtility.SetDirty(assetBundle);
            }
        }
    }
}