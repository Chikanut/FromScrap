using Unity.Entities;
using Unity.Mathematics;

namespace ECS.FindTargetSystem
{
    [GenerateAuthoringComponent]
    public struct HasTarget : IComponentData
    {
        public Entity TargetEntity;
        public float3 TargetPosition;
    }
}
