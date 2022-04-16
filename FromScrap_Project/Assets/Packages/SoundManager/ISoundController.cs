using System.Collections.Generic;
using Packages.Utils.SoundManager.Signals;
using UnityEngine;
using UnityEngine.Audio;

namespace Packages.Utils.SoundManager
{
    public interface ISoundController
    {
        
        string CurrentMusicClip { get; }
        AudioSource MusicSource { set; }
        AudioMixer MasterMixer { set; }
        SoundsConfig SoundConfig { set; }
        bool SoundOn { get; set; }
        bool MusicOn { get; set; }
        float Volume { get; set; }
        AudioMixerGroup EffectsOutputGroup { get; }
        AudioMixerGroup MusicOutputGroup { get; }
        string GetMusicClipNow();
        void SetMusicLowPassCutoff(bool isActive);
        void StopAddressable(StopAudioClipAddressableSignal signal);
        void PlayAddressable(PlayAudioClipAddressableSignal signal);
        public void PlayAudioClipAddressable(PlayAudioClipAddressableSignal clipInfo);
    }
}