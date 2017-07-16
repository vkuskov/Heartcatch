using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Heartcatch.UI.View
{
    public interface IUiSoundPlayer
    {
        void PlaySound(AudioClip clip);
    }
}
