using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartcatch.Services
{
    public sealed class LevelLoaderService : BaseLevelLoaderService
    {
        protected override AsyncOperation loadScene(string path, bool additive)
        {
            return SceneManager.LoadSceneAsync(path, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }
    }
}