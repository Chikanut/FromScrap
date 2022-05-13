using Unity.Entities;

namespace StatisticsSystem.Components
{
    public struct CharacteristicsModificationComponent : IComponentData
    {
        public const int MaxTryUpdateTimes = 60;
        public int CurrentTryUpdateTimes;
        public bool Multiply;
        public Characteristics Value;
    }
}