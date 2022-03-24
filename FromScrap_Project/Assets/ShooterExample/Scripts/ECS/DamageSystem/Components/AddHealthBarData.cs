using Unity.Entities;
using Unity.Mathematics;

namespace DamageSystem.Components
{
    public struct AddHealthBarData : IComponentData
    {
        public float3 Offset;
    }
}