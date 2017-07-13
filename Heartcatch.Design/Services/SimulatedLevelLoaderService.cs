using Heartcatch.Services;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Design.Services
{
    public sealed class SimulatedLevelLoaderService : BaseLevelLoaderService
    {
        protected override AsyncOperation LoadScene(string path, bool additive)
        {
            if (additive)
                return EditorApplication.LoadLevelAdditiveAsyncInPlayMode(path);
            return EditorApplication.LoadLevelAsyncInPlayMode(path);
        }
    }
}
