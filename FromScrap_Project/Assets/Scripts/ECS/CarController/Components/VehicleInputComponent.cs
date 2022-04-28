using Unity.Entities;
using Unity.Mathematics;

namespace Vehicles.Components
{
    public struct VehicleInputComponent : IComponentData
    {
        public float Acceleration;
        public float Steer;
        public float3 MoveDir;
        public float3 MoveDirVelocity;

        public float3 CurrentVelocity;
        public float3 CurrentDirection;
    }
}