using Heartcatch.Core;
using Heartcatch.UI.Models;
using Heartcatch.UI.Services;
using Heartcatch.UI.View;
using UnityEngine;

namespace Heartcatch.UI
{
    public class UIContext : SignalContext
    {
        private const string UIConfigResource = "UIConfig";

        private IScreenManagerService screenManagerService;

        public UIContext(MonoBehaviour view) : base(view)
        {
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            injectionBinder.Bind<IScreenManagerService>().To<ScreenManagerService>().ToSingleton();
            screenManagerService = injectionBinder.GetInstance<IScreenManagerService>();
            injectionBinder.Bind<IUISoundService>().To<UISoundService>().ToSingleton();
            FindAndBindAllScreens();
            BindUISoundManager();
            LoadUIConfig();
        }

        private void LoadUIConfig()
        {
            var soundConfig = Resources.Load<UIConfigModel>(UIConfigResource);
            injectionBinder.Bind<IUIConfigModel>().ToValue(soundConfig);
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