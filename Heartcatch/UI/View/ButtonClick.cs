using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartcatch.UI.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heartcatch.UI.View
{
    [RequireComponent(typeof(Button))]
    public class ButtonClick : strange.extensions.mediation.impl.View
    {
        [Inject]
        public IUISoundService UiSoundService { get; set; }

        protected override void Awake()
        {
            base.Awake();
            var button = GetComponent<Button>();
            button.onClick.AddListener(UiSoundService.OnClick);
        }
    }
}
