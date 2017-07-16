using Heartcatch.Core.Models;
using Heartcatch.Core.Services;
using strange.extensions.mediation.impl;

namespace Heartcatch.Core.View
{
    public sealed class StartupMediator : Mediator
    {
        [Inject]
        public AssetsReadySignal AssetsReady { get; set; }

        [Inject]
        public IGameConfigModel GameConfigModel { get; set; }

        [Inject]
        public ILoaderService LoaderService { get; set; }

        [Inject]
        public ILevelLoaderService LevelLoaderService { get; set; }

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
            LoaderService.LoadAssetBundle(GameConfigModel.FirstSceneBundle, OnTestLevelLoaded);
        }

        private void OnTestLevelLoaded(IAssetBundleModel bundle)
        {
            LevelLoaderService.LoadScenes(bundle.GetScenePath(0));
        }
    }
}
