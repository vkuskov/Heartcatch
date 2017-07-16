using Heartcatch.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Heartcatch.Design.Services
{
    public sealed class SimulatedSceneLoaderService : BaseSceneLoaderService
    {
        protected override AsyncOperation LoadScene(string path, bool additive)
        {
            if (additive)
                return EditorApplication.LoadLevelAdditiveAsyncInPlayMode(path);
            return EditorApplication.LoadLevelAsyncInPlayMode(path);
        }
    }
}