using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Vehicles.Components
{
    public struct VehicleSettingsComponent : IComponentData
    {
        //Input
        public float Acceleration;
        public float Steer;
        public float3 MoveDir;
        
        //Settings
        public float WheelBase;
        public float WheelFrictionRight;
        public float WheelFrictionForward;
        public float WheelMaxImpulseRight;
        public float WheelMaxImpulseForward;
        public float SuspensionLength;
        public float SuspensionStrength;
        public float SuspensionDamping;
        public float MaxSteerAngle;
        public float MaxAcceleration;
        public CollisionFilter CollisionFilter;
        #if UNITY_EDITOR
        //Debug
        public bool DrawDebugInformation;
        #endif
    }
}