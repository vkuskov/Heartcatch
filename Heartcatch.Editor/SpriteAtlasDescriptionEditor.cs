using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Heartcatch.Editor
{
    [CustomEditor(typeof(SpriteAtlasDescription))]
    class SpriteAtlasDescriptionEditor : ContentPreprocessorEditor<SpriteAtlasDescription>
    {
    }
}
