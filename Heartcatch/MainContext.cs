using Heartcatch.Models;
using Heartcatch.Services;
using UnityEngine;
using UnityEngine.Networking;

namespace Heartcatch
{
    public class MainContext : SignalContext
    {
        private const string GAME_CONFIG_RESOURCE = "GameConfig";

        private BaseLevelLoaderService _levelLoaderService;
        private LoaderService _loaderService;
        private SmoothTimeService _timeService;
        private UpdateService _updateService;

        public MainContext(MonoBehaviour view) : base(view)
        {
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            var gameConfig = Resources.Load<GameConfigModel>(GAME_CONFIG_RESOURCE);
            injectionBinder.Bind<IGameConfigModel>().ToValue(gameConfig).CrossContext();
#if UNITY_EDITOR
            if (useSimuationMode())
            {
                _levelLoaderService = new SimulatedLevelLoaderService();
                injectionBinder.Bind<ILoaderService>().To<SimulatedLoaderService>().ToSingleton().CrossContext();
                injectionBinder.Bind<ILevelLoaderService>().ToValue(_levelLoaderService).CrossContext();
            }
            else
            {
                _loaderService = new LoaderService(getServerURL(gameConfig));
                _levelLoaderService = new LevelLoaderService();
                injectionBinder.Bind<ILoaderService>().ToValue(_loaderService).CrossContext();
                injectionBinder.Bind<ILevelLoaderService>().ToValue(_levelLoaderService);
            }
#else
            _loaderService = new LoaderService(getServerURL(gameConfig));
            _levelLoaderService = new LevelLoaderService();
            injectionBinder.Bind<ILoaderService>().ToValue(_loaderService).CrossContext();
            injectionBinder.Bind<ILevelLoaderService>().ToValue(_levelLoaderService);
#endif
            _updateService = new UpdateService();
            _timeService = new SmoothTimeService();
            injectionBinder.Bind<IUpdateService>().ToValue(_updateService).CrossContext();
            injectionBinder.Bind<ITimeService>().ToValue(_timeService).CrossContext();
            injectionBinder.Bind<AssetsReadySignal>().ToSingleton().CrossContext();
        }

        private string getServerURL(IGameConfigModel config)
        {
#if UNITY_EDITOR && !FORCE_REAL_BUNDLES
            IPHostEntry host;
            var localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            return string.Format("http://{0}:7888/{1}/{2}",
                                 localIP,
                                 Utility.ASSET_BUNDLES_OUTPUT_PATH,
                                 Utility.GetPlatformName());

#else
#if LOCAL_BUNDLES
            return string.Format("file://{0}/{1}/{2}", Application.streamingAssetsPath, Utility.ASSET_BUNDLES_OUTPUT_PATH, Utility.GetPlatformName());
#else
#if RELEASE_BUILD
            return string.Format("{0}/bundles/{1}/{2}", config.AssetBundleURL, Utility.ASSET_BUNDLES_OUTPUT_PATH,
                Utility.GetPlatformName());
#else
            return string.Format("{0}/devbundles/{1}/{2}", config.AssetBundleURL, Utility.ASSET_BUNDLES_OUTPUT_PATH,
                Utility.GetPlatformName());
#endif // RELEASE_BUILD 
#endif // LOCAL_BUNDLES
#endif // UNITY_EDITOR && !FORCE_REAL_BUNDLES
        }

        public void Update()
        {
            _timeService.Update(Time.deltaTime);
            _levelLoaderService.Update();
            if (_loaderService != null)
                _loaderService.Update();
            _updateService.Update();
        }

        public void OnAssetsReady()
        {
            var signal = injectionBinder.GetInstance<AssetsReadySignal>();
            signal.Dispatch();
        }

        private bool useSimuationMode()
        {
            return PlayerPrefs.GetInt(Utility.ASSET_BUNDLE_SIMULATION_MODE, 0) != 0;
        }
    }
}