using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SpawnGameObjects.Components
{
    public struct SpawnEntityPoolObjectEvent 
    { 
        public FixedString32Bytes EntityName;
        public float SpawnChance;
        public float3 Position;
    }
}