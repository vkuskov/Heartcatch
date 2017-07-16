using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartcatch.Core.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartcatch.Core.Services
{
    public sealed class LevelLoaderService : ILevelLoaderService
    {
        [Inject]
        public IGameConfigModel GameConfig { get; set; }

        [Inject]
        public ISceneLoaderService SceneLoaderService { get; set; }

        [Inject]
        public IAssetLoaderService AssetLoaderService { get; set; }

        private sealed class ReferenceHack
        {
            public int Index { get; private set; }

            public ReferenceHack(int index)
            {
                Index = index;
            }
        }

        public void LoadLevel(params LevelReference[] parts)
        {
            if (!IsLoadingScreen())
            {
                SceneManager.LoadScene(GameConfig.LoadingScene);
            }
            AssetLoaderService.UnloadAll();
            Resources.UnloadUnusedAssets();
            GC.Collect();
            var sceneNames = new string[parts.Length];
            int bundlesToLoad = parts.Length;
            for (int i = 0; i < parts.Length; ++i)
            {
                var part = parts[i];
                var reference = new ReferenceHack(i);
                AssetLoaderService.LoadAssetBundle(part.AssetBundle, bundle =>
                {
                    sceneNames[reference.Index] = bundle.GetScenePath(part.SceneIndex);
                    bundlesToLoad--;
                    if (bundlesToLoad == 0)
                    {
                        SceneLoaderService.LoadScenes(sceneNames);
                    }
                });
            }
        }

        private bool IsLoadingScreen()
        {
            return SceneManager.sceneCount == 1 && SceneManager.GetSceneAt(0).name == GameConfig.LoadingScene;
        }
    }
}
