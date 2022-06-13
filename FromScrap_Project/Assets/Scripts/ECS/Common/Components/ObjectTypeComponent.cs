using Unity.Entities;

namespace ECS.Common
{
    public enum EntityObjectType
    {
        Unit,
        Object,
        Collectable,
        Player,
        None,
        Projectile
    }
    
    public struct ObjectTypeComponent : ISharedComponentData
    {
        public EntityObjectType Type;
    }
}
