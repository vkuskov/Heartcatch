using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartcatch.Core.Services
{
    public sealed class SceneLoaderService : BaseSceneLoaderService
    {
        protected override AsyncOperation LoadScene(string path, bool additive)
        {
            return SceneManager.LoadSceneAsync(path, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }
    }
}