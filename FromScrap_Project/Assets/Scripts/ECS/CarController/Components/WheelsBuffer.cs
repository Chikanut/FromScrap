using Unity.Entities;

namespace Vehicles.Components
{
    public struct WheelsBuffer : IBufferElementData
    {
        public Entity Wheel;
        public bool isStearing;
        public bool isDriven;
    }
}