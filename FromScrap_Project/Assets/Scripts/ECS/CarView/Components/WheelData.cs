using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Cars.View.Components
{
    public struct WheelData : IComponentData
    {
        public Entity Parent;
        
        [Header("Suspension")]
        public float Radius;
        public float SuspensionDistance;
        public float SuspensionOffset;
        public float SuspensionDamping;
        
        [Header("Guiding")]
        public bool isGuide;
        public bool isLeft;
        [Range(0, 1)]
        public float TurnRange;
        public float TurnDamping;
        
        public float3 LocalAnchor;
        public float3 PrevPos;
        public float3 SuspensionVelocity;
        public float3 TurnVelocity;
        public float3 TurnDirection;
        public float CurrentAngle;
    }
}
