using Unity.Entities;

namespace Kits.Components
{
    public struct KitPlatformComponent : IComponentData
    {
        public Entity Scheme;
        public bool IsFree;
        public bool CanOccupy;
    }
}
