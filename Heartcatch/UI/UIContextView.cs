using Heartcatch.Core.Services;
using Heartcatch.UI.Models;
using Heartcatch.UI.View;
using strange.extensions.context.impl;
using UnityEngine;

namespace Heartcatch.UI
{
    [RequireComponent(typeof(UiSoundManager))]
    public class UiContextView : ContextView
    {
        [Inject]
        public ITimeService TimeService { get; set; }

        [SerializeField] private UiConfigModel uiConfig;

        protected virtual void Awake()
        {
            context = CreateUiContext(uiConfig);
            context.Start();
        }

        protected virtual UiContext CreateUiContext(IUiConfigModel uiConfig)
        {
            return new UiContext(this, uiConfig);
        }

        protected virtual void Update()
        {
            var uiContext = (UiContext) context;
            uiContext.Update(TimeService.Time);
        }
    }
}