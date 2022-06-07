using Unity.Entities;

namespace ECS.Common
{
    public struct QuadrantHashKey : IComponentData
    {
        public int HashKey;
    }
}