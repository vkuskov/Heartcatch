using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartcatch.UI.Models;
using UnityEngine;
using UnityEngine.Audio;

namespace Heartcatch.UI.View
{
    public sealed class UISoundManager : strange.extensions.mediation.impl.View, IUISoundPlayer
    {
        private const int SoundSlots = 3;

        [Inject]
        public IUIConfigModel UiConfigModel { get; set; }

        private List<AudioSource> audioSources = new List<AudioSource>();

        protected override void Start()
        {
            base.Start();
            for (int i = 0; i < SoundSlots; ++i)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = UiConfigModel.UISoundMixer;
                audioSources.Add(source);
            }
        }

        private int FindFreeSourceIndex()
        {
            for (int i = 0; i < audioSources.Count; ++i)
            {
                if (!audioSources[i].isPlaying)
                {
                    return i;
                }
            }
            return audioSources.Count - 1;
        }

        public void PlaySound(AudioClip clip)
        {
            var sourceIndex = FindFreeSourceIndex();
            var source = audioSources[sourceIndex];
            source.PlayOneShot(clip);
            audioSources.RemoveAt(sourceIndex);
            audioSources.Insert(0, source);
        }
    }
}
