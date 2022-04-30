using Unity.Collections;
using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]

    public struct SpawnPoolObjectOnDeathBuffer : IBufferElementData
    {
        public FixedString32Bytes SpawnObjectName;
        public float SpawnChance;
    }
}
