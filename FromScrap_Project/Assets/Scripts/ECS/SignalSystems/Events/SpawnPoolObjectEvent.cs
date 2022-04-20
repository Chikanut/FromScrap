using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SpawnGameObjects.Components
{
    public struct SpawnPoolObjectEvent
    {
        public FixedString32Bytes SpawnObjectName;
        public float3 Position;
    }
}
