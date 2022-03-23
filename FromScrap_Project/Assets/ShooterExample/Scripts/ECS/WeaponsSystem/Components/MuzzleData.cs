using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct MuzzleData : IComponentData
    {
        public float3 Offset;
    }
}
