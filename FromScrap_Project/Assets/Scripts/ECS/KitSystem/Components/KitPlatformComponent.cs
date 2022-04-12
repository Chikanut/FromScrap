using Unity.Entities;

namespace Kits.Components
{
    public struct KitPlatformComponent : IComponentData
    {
        public bool IsFree;
        public bool CanOccupy;
    }
}
