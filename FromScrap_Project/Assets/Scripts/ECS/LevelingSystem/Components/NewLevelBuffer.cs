using Unity.Entities;

namespace LevelingSystem.Components
{
    public struct NewLevelBuffer : IBufferElementData
    {
        public int Level;
    }
}