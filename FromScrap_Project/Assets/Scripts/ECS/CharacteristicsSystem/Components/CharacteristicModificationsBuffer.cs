using Unity.Entities;

namespace StatisticsSystem.Components
{
    public struct CharacteristicModificationsBuffer : IBufferElementData
    {
        public Entity ModificatorHolder;
        public Characteristics Value;
    }
}