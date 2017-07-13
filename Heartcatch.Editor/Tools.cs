using UnityEditor;
using UnityEngine;

namespace Heartcatch.Editor
{
    public class Tools
    {
        [MenuItem("Heartcatch/Clear PlayerPerfs", priority = 1)]
        public static void ClearPlayerPerf()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Heartcatch/Clear bundle cache", priority = 2)]
        public static void ClearBundleCache()
        {
            Caching.CleanCache();
        }
    }
}