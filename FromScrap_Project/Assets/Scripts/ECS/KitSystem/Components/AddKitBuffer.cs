using Unity.Entities;

namespace Kits.Components
{
    public struct AddKitBuffer : IBufferElementData
    {
        public int CarID;
        public int PlatformID;
        public int KitID;
        public int KitLevel;
    }
}