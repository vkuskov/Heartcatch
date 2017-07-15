using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Heartcatch.UI.View
{
    public interface IUISoundPlayer
    {
        void PlaySound(AudioClip clip);
    }
}
