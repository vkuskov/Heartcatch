using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace Heartcatch.UI.Models
{
    public interface IUIConfigModel
    {
        AudioMixerGroup UISoundMixer { get; }
        AudioClip ButtonClickSound { get; }
        AudioClip DenySound { get; }
        AudioClip AcceptSound { get; }
    }
}
