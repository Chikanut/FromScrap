using Unity.Entities;

namespace Magnet.Components
{
    public struct MagnetTargetsBuffer : IBufferElementData
    {
        public Entity Target;
    }
}