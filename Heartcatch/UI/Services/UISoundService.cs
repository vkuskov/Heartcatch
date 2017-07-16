using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartcatch.UI.Models;
using Heartcatch.UI.View;
using UnityEngine;

namespace Heartcatch.UI.Services
{
    public sealed class UiSoundService : IUiSoundService
    {
        [Inject]
        public IUiConfigModel UiConfigModel { get; set; }

        [Inject]
        public IUiSoundPlayer UiSoundPlayer { get; set; }

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
