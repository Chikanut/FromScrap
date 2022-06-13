using Unity.Entities;
using Unity.Mathematics;

namespace ForceField.Components
{
    public struct AddForceComponent : IComponentData
    {
        public float Force;
        public float3 Dir;
    }
}