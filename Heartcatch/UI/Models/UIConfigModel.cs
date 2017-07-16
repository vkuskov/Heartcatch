using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace Heartcatch.UI.Models
{
    [CreateAssetMenu(menuName = "Heartcatch/UI Config")]
    public sealed class UiConfigModel : ScriptableObject, IUiConfigModel
    {
        [SerializeField] private AudioMixerGroup uiSoundMixer;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip denySound;
        [SerializeField] private AudioClip acceptSound;

        public AudioMixerGroup UiSoundMixer
        {
            get { return uiSoundMixer; }
        }

        public AudioClip ButtonClickSound
        {
            get { return buttonClickSound; }
        }

        public AudioClip DenySound
        {
            get { return denySound; }
        }

        public AudioClip AcceptSound
        {
            get { return acceptSound; }
        }
    }
}
