using Heartcatch.Core.Models;
using Heartcatch.Core.Services;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartcatch.Core.View
{
    public sealed class StartupMediator : Mediator
    {
        [Inject]
        public StartupView View { get; set; }

        [Inject]
        public AssetsReadySignal AssetsReady { get; set; }

        [Inject]
        public IGameConfigModel GameConfigModel { get; set; }

        [Inject]
        public ILevelLoaderService LevelLoaderService { get; set; }

        [Inject]
        public IAssetLoaderService AssetLoaderService { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            SceneManager.LoadScene(GameConfigModel.LoadingScene);
            AssetsReady.AddListener(OnAssetsReady);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            AssetsReady.RemoveListener(OnAssetsReady);
        }

        private void OnAssetsReady()
        {
            Debug.Log("Preload asset bundles...");
            AssetLoaderService.Preload(View.PreloadedBundles, OnBundlesPreloaded);
        }

        private void OnBundlesPreloaded()
        {
            Debug.Log("Loading first level...");
            LevelLoaderService.LoadLevel(GameConfigModel.FirstLevel);
        }
    }
}
