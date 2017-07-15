using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartcatch.UI.Models;
using Heartcatch.UI.View;
using UnityEngine;

namespace Heartcatch.UI.Services
{
    public sealed class UISoundService : IUISoundService
    {
        [Inject]
        public IUIConfigModel UiConfigModel { get; set; }

        [Inject]
        public IUISoundPlayer UiSoundPlayer { get; set; }

        public void OnClick()
        {
            UiSoundPlayer.PlaySound(UiConfigModel.ButtonClickSound);
        }

        public void OnDeny()
        {
            UiSoundPlayer.PlaySound(UiConfigModel.DenySound);
        }

        public void OnAccept()
        {
            UiSoundPlayer.PlaySound(UiConfigModel.AcceptSound);
        }
    }
}
