using System;
using Heartcatch.Core;
using Heartcatch.UI.Models;
using Heartcatch.UI.Services;
using UnityEngine;

namespace Heartcatch.UI.View
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    public abstract class ScreenView : strange.extensions.mediation.impl.View, IScreenModel
    {
        private const float TransitionCurve = 2.2f;

        private CanvasGroup canvasGroup;
        private Canvas canvas;
        private bool otherScreenHasFocus;
        private ScreenState screenState = ScreenState.Hidden;
        private TimeSpan transitionOffTime = TimeSpan.Zero;
        private TimeSpan transitionOnTime = TimeSpan.Zero;
        private float transitionPosition = 1f;

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
            IsExiting = false;
            screenState = ScreenState.TransitionOn;
            OnAttached();
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
                }
            }
            canvas.enabled = screenState != ScreenState.Hidden;
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
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponent<Canvas>();
            base.Awake();
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta;
            if (time == TimeSpan.Zero)
                transitionDelta = 1f;
            else
                transitionDelta = (float) (gameTime.UnscaledDeltaTime / time.TotalSeconds);
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
            canvasGroup.alpha = Mathf.Pow(1f - position, TransitionCurve);
        }

        protected virtual void OnScreenRemoved()
        {
            canvas.enabled = false;
            gameObject.SetActive(false);
        }

        protected virtual void OnAttached()
        {
            gameObject.SetActive(true);
            canvas.enabled = true;
        }
    }
}