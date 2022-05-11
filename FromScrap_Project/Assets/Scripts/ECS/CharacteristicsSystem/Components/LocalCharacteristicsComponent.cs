using Unity.Entities;

namespace StatisticsSystem.Components
{
    public struct LocalCharacteristicsComponent : IComponentData
    {
        public Characteristics Value;
    }
}