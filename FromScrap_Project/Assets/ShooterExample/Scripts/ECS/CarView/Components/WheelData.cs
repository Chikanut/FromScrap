using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Cars.View.Components
{
    public struct WheelData : IComponentData
    {
        public float Radius;
        public float SuspensionDistance;
        public float SuspensionOffset;
        public float SuspensionDamping;
        
        public bool isGuide;
        [Range(0, 1)]
        public float TurnRange;
        public float3 TurnDirection;
        public float TurnDamping;
        public float3 TurnVelocity;
        
        public float3 Anchor;
        public float3 PrevPos;
        public float3 Velocity;

        public float3 ParentUp;
        public float3 ParentRight;
    }
}
