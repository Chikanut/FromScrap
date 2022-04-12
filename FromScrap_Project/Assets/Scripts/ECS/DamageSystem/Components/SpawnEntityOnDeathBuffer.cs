using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    public struct SpawnEntityOnDeathBuffer : IBufferElementData
    {
        public Entity SpawnEntity;
    }
}