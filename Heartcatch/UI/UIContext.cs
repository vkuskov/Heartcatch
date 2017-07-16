using Heartcatch.Core;
using Heartcatch.UI.Models;
using Heartcatch.UI.Services;
using Heartcatch.UI.View;
using UnityEngine;

namespace Heartcatch.UI
{
    public class UIContext : SignalContext
    {
        private IScreenManagerService screenManagerService;
        private IUIConfigModel uiConfig;

        public UIContext(MonoBehaviour view, IUIConfigModel uiConfig) : base(view)
        {
            this.uiConfig = uiConfig;
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            injectionBinder.Bind<IScreenManagerService>().To<ScreenManagerService>().ToSingleton();
            screenManagerService = injectionBinder.GetInstance<IScreenManagerService>();
            injectionBinder.Bind<IUISoundService>().To<UISoundService>().ToSingleton();
            injectionBinder.Bind<IUIConfigModel>().ToValue(uiConfig);
            FindAndBindAllScreens();
            BindUISoundManager();
        }

        private void BindUISoundManager()
        {
            var go = contextView as GameObject;
            injectionBinder.Bind<IUISoundPlayer>().ToValue(go.GetComponent<IUISoundPlayer>());
        }

        private void FindAndBindAllScreens()
        {
            var root = contextView as GameObject;
            var allScreens = root.GetComponentsInChildren<ScreenView>(true);
            foreach (var screen in allScreens)
                injectionBinder.Bind(screen.GetType()).ToValue(screen);
        }

        public void Update(GameTime gameTime)
        {
            screenManagerService.Update(gameTime);
        }
    }
}