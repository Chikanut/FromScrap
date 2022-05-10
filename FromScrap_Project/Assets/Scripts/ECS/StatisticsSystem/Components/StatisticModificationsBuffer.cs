using Unity.Entities;

namespace StatisticsSystem.Components
{
    public struct StatisticModificationsBuffer : IBufferElementData
    {
        public Entity ModificatorHolder;
        public Statistics Modificator;
    }
}