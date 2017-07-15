using System;
using Heartcatch.Core;
using Heartcatch.UI.Models;
using Heartcatch.UI.Services;
using UnityEngine;

namespace Heartcatch.UI.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenView : strange.extensions.mediation.impl.View, IScreenModel
    {
        private CanvasGroup canvasGroup;
        private bool otherScreenHasFocus;
        private ScreenState screenState = ScreenState.TransitionOn;
        private TimeSpan transitionOffTime = TimeSpan.Zero;
        private TimeSpan transitionOnTime = TimeSpan.Zero;
        private float transitionPosition = 1f;

        public ScreenView()
        {
            IsPopup = false;
            IsExiting = false;
        }

        protected IScreenManagerService ScreenManagerService { get; private set; }

        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        public bool IsExiting { get; private set; }

        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus && (screenState == ScreenState.TransitionOn ||
                                                screenState == ScreenState.Active);
            }
        }

        public bool IsPopup { get; protected set; }

        public ScreenState State
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        public void Attach(IScreenManagerService screenManagerService)
        {
            transform.SetAsLastSibling();
            ScreenManagerService = screenManagerService;
            OnAttached(screenManagerService);
            IsExiting = false;
            screenState = ScreenState.TransitionOn;
        }

        public void OnUpdate(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;
            if (IsExiting)
            {
                screenState = ScreenState.TransitionOff;
                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    ScreenManagerService.RemoveScreen(this);
                    OnScreenRemoved();
                }
            }
            else if (coveredByOtherScreen)
            {
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    screenState = ScreenState.Hidden;
                    OnScreenHide();
                }
            }
            else
            {
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    screenState = ScreenState.Active;
                    OnScreenShow();
                }
            }
        }

        public void ExitScreen()
        {
            if (transitionOffTime == TimeSpan.Zero)
            {
                ScreenManagerService.RemoveScreen(this);
                OnScreenRemoved();
            }
            else
            {
                IsExiting = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta;
            if (time == TimeSpan.Zero)
                transitionDelta = 1f;
            else
                transitionDelta = (float) (gameTime.ElapsedTime.TotalSeconds / time.TotalSeconds);
            transitionPosition += transitionDelta * direction;
            if (direction < 0 && transitionPosition <= 0 || direction > 0 && transitionPosition >= 1)
            {
                transitionPosition = Mathf.Clamp01(transitionPosition);
                OnUpdateTransition(transitionPosition);
                return false;
            }
            OnUpdateTransition(transitionPosition);
            return true;
        }

        protected virtual void OnUpdateTransition(float position)
        {
            canvasGroup.alpha = 1f - position;
        }

        protected virtual void OnScreenRemoved()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnScreenHide()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnScreenShow()
        {
            gameObject.SetActive(true);
        }

        protected virtual void OnAttached(IScreenManagerService screenManagerService)
        {
            gameObject.SetActive(true);
        }
    }
}