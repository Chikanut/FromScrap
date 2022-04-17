using System;
using ShootCommon.Signals;
using UnityEngine;

namespace Packages.Utils.SoundManager.Signals
{
    public enum SoundType
    {
        Default,
        Music,
        Loop,
        Singleton,
        Delayed
    }
    
    public class PlayAudioClipAddressableSignal : Signal
    {
        public string ClipName;
        public SoundType ClipType = SoundType.Default;
        public float Delay = 0;
        public float Pitch = 1;
        public float PitchTime = 0;
        public Vector3? Position;
        public Action<AudioSource> Source;
    }

    public class StopAudioClipAddressableSignal : Signal
    {
        public AudioSource ClipSource;
    }
}