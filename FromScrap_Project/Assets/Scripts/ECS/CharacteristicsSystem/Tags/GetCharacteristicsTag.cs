using Unity.Entities;

namespace StatisticsSystem.Tags
{
    public struct GetCharacteristicsTag : IComponentData
    {
        public const int MaxTryUpdateTimes = 60;
        public int TryUpdateTimes;
    }
}