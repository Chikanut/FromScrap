using Unity.Entities;

namespace StatisticsSystem.Components
{
    public struct LocalStatisticsComponent : IComponentData
    {
        public Statistics Value;
    }
}