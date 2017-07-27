using System;
using System.Collections.Generic;
using Heartcatch.Core;
using Heartcatch.UI.Models;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Heartcatch.UI.Services
{
    public sealed class ScreenManagerService : IScreenManagerService
    {
        private readonly List<IScreenModel> screens = new List<IScreenModel>();
        private readonly List<IScreenModel> screensToUpdate = new List<IScreenModel>();

        public void AddScreen(IScreenModel screen)
        {
            if (screens.Contains(screen))
            {
                throw new ArgumentException(string.Format("Screen {0} is already added", screen));
            }
            screen.Attach(this);
            screens.Add(screen);
        }

        public void RemoveScreen(IScreenModel screen)
        {
            if (!screens.Contains(screen))
            {
                throw new ArgumentException(string.Format("Screen {0} wasn't registered"));
            }
            screensToUpdate.Remove(screen);
            screens.Remove(screen);
        }

        public void Update(GameTime gameTime, bool trace)
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
            if (trace)
            {
                Trace();
            }
        }

        public void Trace()
        {
            Debug.Log("======== Screen Manager Trace ====");
            for (int i = 0; i < screens.Count; ++i)
            {
                var screen = screens[i];
                Debug.LogFormat("{0} - Popup: {1} - State: {2} - Transition: {3} - {4}", i + 1, screen.IsPopup, screen.State, screen.TransitionPosition, screen);
            }
        }

    }
}