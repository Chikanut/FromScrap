using StatisticsSystem.Components;
using Unity.Entities;

namespace CharacteristicsPerLevelSystem.Components
{
    public struct UpgradesPerLevelBuffer : IBufferElementData
    {
        public Characteristics Characteristics;
    }
}