using Unity.Entities;

namespace StatisticsSystem.Components
{
    public struct CharacteristicsModificationComponent : IComponentData
    {
        public Characteristics Value;
    }
}