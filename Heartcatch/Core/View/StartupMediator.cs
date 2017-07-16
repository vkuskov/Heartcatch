﻿using Heartcatch.Core.Models;
using Heartcatch.Core.Services;
using strange.extensions.mediation.impl;
using UnityEngine;

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
        public ILoaderService LoaderService { get; set; }

        [Inject]
        public ISceneLoaderService SceneLoaderService { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
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
            LoaderService.Preload(View.PreloadedBundles, OnBundlesPreloaded);
        }

        private void OnBundlesPreloaded()
        {
            LoaderService.LoadAssetBundle(GameConfigModel.FirstSceneBundle, OnTestLevelLoaded);
        }

        private void OnTestLevelLoaded(IAssetBundleModel bundle)
        {
            SceneLoaderService.LoadScenes(bundle.GetScenePath(0));
        }
    }
}