using Unity.Entities;

namespace UpgradesSystem.Components
{
    public struct UpgradeComponent : IComponentData
    {
        public Entity Target;
    }
}