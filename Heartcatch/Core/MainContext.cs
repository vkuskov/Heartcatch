﻿using System.Net;
using System.Net.Sockets;
using Heartcatch.Core.Models;
using Heartcatch.Core.Services;
using Heartcatch.Core.View;
using strange.extensions.pool.api;
using strange.extensions.pool.impl;
using UnityEngine;

namespace Heartcatch.Core
{
    public abstract class MainContext : SignalContext
    {
        private IAssetLoaderService baseAssetLoaderService;
        private ISceneLoaderService sceneLoaderService;
        private SmoothTimeService timeService;
        private UpdateService updateService;

        public MainContext(MonoBehaviour view) : base(view)
        {
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            var gameConfig = Resources.Load<GameConfigModel>(Utility.GameConfigResource);
            injectionBinder.Bind<IGameConfigModel>().ToValue(gameConfig).CrossContext();
            baseAssetLoaderService = CreateAssetLoaderService(gameConfig);
            sceneLoaderService = CreateSceneLoaderService();
            injectionBinder.Bind<IAssetLoaderService>().ToValue(baseAssetLoaderService).CrossContext();
            injectionBinder.Bind<ISceneLoaderService>().ToValue(sceneLoaderService);

            updateService = new UpdateService();
            timeService = new SmoothTimeService();
            injectionBinder.Bind<IUpdateService>().ToValue(updateService).CrossContext();
            injectionBinder.Bind<ITimeService>().ToValue(timeService).CrossContext();
            injectionBinder.Bind<IResourceLoaderService>().To<ResourceLoaderService>().ToSingleton().CrossContext();
            injectionBinder.Bind<ILevelLoaderService>().To<LevelLoaderService>().ToSingleton().CrossContext();

            injectionBinder.Bind<ResourceRequestModel>().To<ResourceRequestModel>();
            injectionBinder.Bind<IPool<ResourceRequestModel>>().To<Pool<ResourceRequestModel>>().ToSingleton();

            mediationBinder.Bind<StartupView>().ToMediator<StartupMediator>();

            injectionBinder.Bind<AssetsReadySignal>().ToSingleton().CrossContext();
        }

        private string GetServerUrl(IGameConfigModel config)
        {
            if (Application.isEditor)
                return GetLocalServerUrl();
            if (config.IsDevelopmentMode)
                return GetDevelopmentCloudServerUrl(config);
            return GetCloudServerUrl(config);
        }

        public void Update()
        {
            timeService.Update(Time.deltaTime);
            sceneLoaderService.Update();
            if (baseAssetLoaderService != null)
                baseAssetLoaderService.Update();
            updateService.Update();
        }

        public void OnAssetsReady()
        {
            var signal = injectionBinder.GetInstance<AssetsReadySignal>();
            signal.Dispatch();
        }

        protected bool UseSimuationMode()
        {
            return PlayerPrefs.GetInt(Utility.AssetBundleSimulationMode, 0) != 0;
        }

        protected string GetLocalServerUrl()
        {
            IPHostEntry host;
            var localIp = string.Empty;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                    break;
                }
            return string.Format("http://{0}:7888/{1}/{2}",
                localIp,
                Utility.AssetBundlesOutputPath,
                Utility.GetPlatformName());
        }

        protected string GetCloudServerUrl(IGameConfigModel config)
        {
            return string.Format("{0}/bundles/{1}/{2}", config.AssetBundleUrl, Utility.AssetBundlesOutputPath,
                Utility.GetPlatformName());
        }

        protected string GetDevelopmentCloudServerUrl(IGameConfigModel config)
        {
            return string.Format("{0}/devbundles/{1}/{2}", config.AssetBundleUrl, Utility.AssetBundlesOutputPath,
                Utility.GetPlatformName());
        }

        protected abstract IAssetLoaderService CreateAssetLoaderService(IGameConfigModel config);

        protected abstract ISceneLoaderService CreateSceneLoaderService();
    }
}