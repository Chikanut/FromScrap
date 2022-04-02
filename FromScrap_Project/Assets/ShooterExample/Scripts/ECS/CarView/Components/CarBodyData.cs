using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Cars.View.Components
{
    public struct CarBodyData : IComponentData
    {
        public Entity Parent;
        
        
        public float3 Anchor;
        public float RotationDamping;
        public float MovementDamping;
        //
        public float3 CurrentForward;
        public float3 ForwardVelocity;
        public float3 CurrentUp;
        public float3 UpVelocity;

        // public float3 GroundNormal;
        // public float3 GroundForward;
        // public float3 ParentForward;
    }
}