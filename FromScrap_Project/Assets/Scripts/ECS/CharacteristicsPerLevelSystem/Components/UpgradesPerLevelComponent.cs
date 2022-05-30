using StatisticsSystem.Components;
using Unity.Entities;

namespace CharacteristicsPerLevelSystem.Components
{
    [GenerateAuthoringComponent]
    public struct UpgradesPerLevelComponent : IComponentData
    {
        public Characteristics Characteristics;
    }
}