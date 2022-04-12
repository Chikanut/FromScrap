using Unity.Entities;

namespace LevelingSystem.Components
{
    public struct LevelsInfoBuffer : IBufferElementData
    {
        public int TargetExperience;
    }
}