using UnityEditor;
using UnityEngine;

namespace Heartcatch.Editor
{
    public class Tools
    {
        [MenuItem("Tools/Clear PlayerPerfs")]
        public static void ClearPlayerPerf()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Tools/Clear bundle cache")]
        public static void ClearBundleCache()
        {
            Caching.CleanCache();
        }
    }
}