using Unity.Entities;

namespace CharacteristicsPerLevelSystem.Components
{
    public struct UpgradePerLevelListener : IComponentData
    {
        public Entity Target;
    }
}