
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ShipGame.Core.Services
{
    public sealed class SimulatedLevelLoaderService : BaseLevelLoaderService
    {
        protected override AsyncOperation loadScene(string path, bool additive)
        {
            if (additive)
                return EditorApplication.LoadLevelAdditiveAsyncInPlayMode(path);
            return EditorApplication.LoadLevelAsyncInPlayMode(path);
        }
    }
}
#endif