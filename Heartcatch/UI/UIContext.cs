using Heartcatch.Core;
using Heartcatch.UI.Models;
using Heartcatch.UI.Services;
using Heartcatch.UI.View;
using UnityEngine;

namespace Heartcatch.UI
{
    public class UiContext : SignalContext
    {
        private IScreenManagerService screenManagerService;
        private IUiConfigModel uiConfig;

        public UiContext(MonoBehaviour view, IUiConfigModel uiConfig) : base(view)
        {
            this.uiConfig = uiConfig;
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            injectionBinder.Bind<IScreenManagerService>().To<ScreenManagerService>().ToSingleton();
            screenManagerService = injectionBinder.GetInstance<IScreenManagerService>();
            injectionBinder.Bind<IUiSoundService>().To<UiSoundService>().ToSingleton();
            injectionBinder.Bind<IUiConfigModel>().ToValue(uiConfig);
            FindAndBindAllScreens();
            BindUiSoundManager();
        }

        private void BindUiSoundManager()
        {
            var go = contextView as GameObject;
            injectionBinder.Bind<IUiSoundPlayer>().ToValue(go.GetComponent<IUiSoundPlayer>());
        }

        private void FindAndBindAllScreens()
        {
            var root = contextView as GameObject;
            var allScreens = root.GetComponentsInChildren<ScreenView>(true);
            foreach (var screen in allScreens)
                injectionBinder.Bind<ScreenView>().ToValue(screen).ToName(screen.GetType());
        }

        public void Update(GameTime gameTime)
        {
            screenManagerService.Update(gameTime);
        }
    }
}