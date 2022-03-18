
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS_Test
{
    public struct MovementComponent : IComponentData
    {
        public float3 Velocity;
    }
}
