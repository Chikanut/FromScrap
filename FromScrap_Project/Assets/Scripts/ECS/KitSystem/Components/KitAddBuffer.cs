using Unity.Entities;

namespace Kits.Components
{
    public struct KitAddBuffer : IBufferElementData
    {
        public int CarID;
        public int PlatformID;
        public int KitID;
        public int KitLevel;
    }
}