using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartcatch.Design.Models;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Editor
{
    [CustomEditor(typeof(UiAssetBundleDescriptionModel))]
    public sealed class UiAssetBundleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Apply"))
            {
                var bundleDescription = (UiAssetBundleDescriptionModel) target;
                bundleDescription.ApplySetting();
            }
        }
    }
}
