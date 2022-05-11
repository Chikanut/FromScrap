using Unity.Entities;

namespace StatisticsSystem.Components
{
    public struct StatisticsModificationComponent : IComponentData
    {
        public Statistics Value;
    }
}