using Heartcatch.Core.Services;
using Heartcatch.UI.Models;
using Heartcatch.UI.View;
using strange.extensions.context.impl;
using UnityEngine;

namespace Heartcatch.UI
{
    public class UiContextView : ContextView
    {
        [Inject]
        public ITimeService TimeService { get; set; }


        protected virtual void Awake()
        {
            context = CreateUiContext();
            context.Start();
        }

        protected virtual UiContext CreateUiContext()
        {
            return new UiContext(this);
        }

        protected virtual void Update()
        {
            var uiContext = (UiContext) context;
            uiContext.Update(TimeService.Time);
        }
    }
}