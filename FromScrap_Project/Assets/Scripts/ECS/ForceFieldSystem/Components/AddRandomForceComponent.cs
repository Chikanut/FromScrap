using Unity.Entities;
using Unity.Mathematics;

namespace ForceField.Components
{
    public struct AddRandomForceComponent : IComponentData
    {
        public float3 Direction;
        public float Spray;
        public float Force;
    }
}