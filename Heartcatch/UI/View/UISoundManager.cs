using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heartcatch.UI.Models;
using UnityEngine;
using UnityEngine.Audio;

namespace Heartcatch.UI.View
{
    public sealed class UiSoundManager : strange.extensions.mediation.impl.View, IUiSoundPlayer
    {
        private const int SoundSlots = 3;

        [Inject]
        public IUiConfigModel UiConfigModel { get; set; }

        private List<AudioSource> audioSources = new List<AudioSource>();

        protected override void Start()
        {
            base.Start();
            for (int i = 0; i < SoundSlots; ++i)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = UiConfigModel.UiSoundMixer;
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
