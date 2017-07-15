using Heartcatch.Core;
using Heartcatch.UI.Services;
using Heartcatch.UI.View;
using UnityEngine;

namespace Heartcatch.UI
{
    public class UIContext : SignalContext
    {
        private IScreenManagerService screenManagerService;

        public UIContext(MonoBehaviour view) : base(view)
        {
        }

        protected override void mapBindings()
        {
            base.mapBindings();
            injectionBinder.Bind<IScreenManagerService>().To<ScreenManagerService>().ToSingleton();
            screenManagerService = injectionBinder.GetInstance<IScreenManagerService>();
            FindAndBindAllScreens();
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