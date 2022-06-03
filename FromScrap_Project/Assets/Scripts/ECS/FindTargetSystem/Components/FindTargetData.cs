using ECS.Common;
using Unity.Entities;

namespace ECS.FindTargetSystem
{
    public struct FindTargetData : ISharedComponentData
    {
        public EntityObjectType TargetType;
    }
}
