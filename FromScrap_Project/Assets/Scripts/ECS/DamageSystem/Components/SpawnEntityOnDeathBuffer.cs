using Unity.Collections;
using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    public struct SpawnEntityOnDeathBuffer : IBufferElementData
    {
        public FixedString32Bytes SpawnEntity;
        public float SpawnChance;
    }
}