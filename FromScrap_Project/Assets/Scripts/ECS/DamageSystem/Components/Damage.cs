using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    public struct Damage : IBufferElementData
    {
        public int Value;
    }
}