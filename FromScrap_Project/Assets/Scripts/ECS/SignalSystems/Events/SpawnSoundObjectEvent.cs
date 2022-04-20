using Packages.Utils.SoundManager.Signals;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SpawnGameObjects.Components
{
    public struct SpawnSoundObjectEvent
    {
        public FixedString32Bytes ClipName;
        public SoundType ClipType;
        public float Delay;
        public float Pitch;
        public float PitchTime;
        public float3? Position;
    }
}