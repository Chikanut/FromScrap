using Unity.Entities;

namespace Kits.Components
{
    public struct KitPlatformKitsBuffer : IBufferElementData
    {
        public Entity ConnectedKit;
    }
}
