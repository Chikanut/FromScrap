using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SpawnGameObjects.Components
{
    public struct SpawnEntityPoolObjectEvent : IComponentData
    { 
        public FixedString32Bytes EntityName;
        public float3 Position;
    }
}