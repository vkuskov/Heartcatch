using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heartcatch.Core
{
    [Serializable]
    public struct LevelReference
    {
        public string AssetBundle;
        public int SceneIndex;

        public LevelReference(string assetBundle, int sceneIndex = 0)
        {
            AssetBundle = assetBundle;
            SceneIndex = sceneIndex;
        }
    }
}
