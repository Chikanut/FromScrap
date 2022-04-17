using Packages.Utils.SoundManager.Signals;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DamageSystem.Components
{
    public struct SpawnSoundObjectOnDeathBuffer : IBufferElementData
    {
        public FixedString32Bytes ClipName;
        public float Delay;
        public SoundType Type;
        public float Pitch;
        public float PitchTime;
    }
}