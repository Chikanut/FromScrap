using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Vehicles.Wheels.Components
{
    public struct ViewData : IComponentData
    {
        public Entity Body;
        public Entity Parent;
        
        public float Radius;
        public bool IsGuide;
        public bool IsDrive;
        public float TurnRange;
        public float TurnDamping;
        
        public float SteerVelocity;
        public float SteeringAngle;
        public float CurrentAngle;
        public float3 PrevPosition;
    }

    public struct SuspensionData : IComponentData
    {        
        public Entity Body;
        public Entity Parent;
        
        public float Radius;
        public float SuspensionDistance;
        public float SuspensionStrength;
        public float SuspensionDamping;
    }

    public struct DriveData : IComponentData
    {
        public Entity Body;
        public Entity Parent;

        public bool IsDrive;
        public bool IsGuide;
        public bool IsSubGuide;
        public float MaxSpeed;
        public float Acceleration;
        public float MaxAcceleration;
        public float MaxSidewaysImpulse;

        public float MaxSteerAngle;
        public float SteerSensivity;
    }
}
