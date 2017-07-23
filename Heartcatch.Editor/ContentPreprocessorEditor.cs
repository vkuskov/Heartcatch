using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Heartcatch.Editor
{
    public abstract class ContentPreprocessorEditor<T> : UnityEditor.Editor where T : ScriptableObject, IContentPreprocessor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Apply"))
            {
                var preprocessor = target as IContentPreprocessor;
                if (target != null)
                {
                    preprocessor.Apply();
                }
            }
        }
    }
}
