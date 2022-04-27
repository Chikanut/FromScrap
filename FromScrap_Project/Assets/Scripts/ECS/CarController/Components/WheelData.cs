using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Cars.View.Components
{
    public struct WheelData : IComponentData
    {
        public Entity Body;
        public Entity Parent;
        
        [Header("Suspension")]
        public float Radius;
        public float SuspensionDistance;
        public float SuspensionStrength;
        public float SuspensionDamping;
        
        [Header("Guiding")]
        public bool isGuide;
        [Range(0, 1)]
        public float TurnRange;
        public float TurnDamping;
        
        public float3 PrevPos;
        public float3 TurnVelocity;
        public float3 TurnDirection;
        public float CurrentAngle;
    }
}
