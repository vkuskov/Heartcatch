using Heartcatch.Core.Services;
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

        protected virtual void Awake()
        {
            context = CreateUIContext();
            context.Start();
        }

        protected virtual UIContext CreateUIContext()
        {
            return new UIContext(this);
        }

        protected virtual void Update()
        {
            var uiContext = (UIContext) context;
            uiContext.Update(TimeService.Time);
        }
    }
}