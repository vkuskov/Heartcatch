using Heartcatch.Models;
using Heartcatch.Services;
using strange.extensions.context.impl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartcatch
{
    public abstract class Runner : ContextView
    {
        public const string CachePrimedFlag = "__cachePrimed";
        private const string CachePrimeScene = "CachePrime";

        private bool isInitialized;

        [Inject]
        public ILoaderService LoaderService { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void StartUp()
        {
            var gameConfig = Resources.Load<GameConfigModel>(Utility.GameConfigResource);

            var cachePrimed = false;
            if (gameConfig.IsLocalBuild)
            {
                cachePrimed = true;
            }
            else
            {
                cachePrimed = PlayerPrefs.GetInt(CachePrimedFlag, 0) != 0;
            }
            if (cachePrimed)
            {
                var runner = Resources.Load<Runner>(Utility.RunnerResource);
                if (runner != null)
                {
                    var go = Instantiate(runner);
                    DontDestroyOnLoad(go.gameObject);
                }
            }
            else
            {
                SceneManager.LoadScene(CachePrimeScene);
            }
        }

        protected virtual void Awake()
        {
            context = CreateMainContext();
            context.Start();
            isInitialized = false;
        }

        protected virtual void Update()
        {
            var mainContext = context as MainContext;
            if (!isInitialized && LoaderService.IsInitialized)
            {
                mainContext.OnAssetsReady();
                isInitialized = true;
            }
            if (mainContext != null)
                mainContext.Update();
        }

        protected abstract MainContext CreateMainContext();
    }
}
