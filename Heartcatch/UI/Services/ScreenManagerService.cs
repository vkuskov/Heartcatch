using System.Collections.Generic;
using Heartcatch.Core;
using Heartcatch.UI.Models;
using UnityEngine.Assertions;

namespace Heartcatch.UI.Services
{
    public sealed class ScreenManagerService : IScreenManagerService
    {
        private readonly List<IScreenModel> screens = new List<IScreenModel>();
        private readonly List<IScreenModel> screensToUpdate = new List<IScreenModel>();

        public void AddScreen(IScreenModel screen)
        {
            Assert.IsFalse(screens.Contains(screen));
            screen.Attach(this);
            screens.Add(screen);
        }

        public void RemoveScreen(IScreenModel screen)
        {
            screensToUpdate.Remove(screen);
            screens.Remove(screen);
        }

        public void Update(GameTime gameTime)
        {
            screensToUpdate.Clear();
            foreach (var screen in screens)
                screensToUpdate.Add(screen);
            var coveredByOtherScreen = false;
            var otherScreenHasFocus = false;
            while (screensToUpdate.Count > 0)
            {
                var screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);
                screen.OnUpdate(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                if (screen.State == ScreenState.TransitionOn || screen.State == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                        otherScreenHasFocus = true;
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }
    }
}