using Heartcatch.Core.Services;
using Heartcatch.UI.Models;
using Heartcatch.UI.View;
using strange.extensions.context.impl;
using UnityEngine;

namespace Heartcatch.UI
{
    [RequireComponent(typeof(UISoundManager))]
    public class UIContextView : ContextView
    {
        [Inject]
        public ITimeService TimeService { get; set; }

        [SerializeField] private UIConfigModel uiConfig;

        protected virtual void Awake()
        {
            context = CreateUIContext();
            context.Start();
        }

        protected virtual UIContext CreateUIContext()
        {
            return new UIContext(this, uiConfig);
        }

        protected virtual void Update()
        {
            var uiContext = (UIContext) context;
            uiContext.Update(TimeService.Time);
        }
    }
}