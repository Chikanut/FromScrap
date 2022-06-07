using Unity.Entities;

namespace ECS.FindTargetSystem
{
    public struct TargetSearchRadiusComponent : IComponentData
    {
        public float Radius;
    }
}