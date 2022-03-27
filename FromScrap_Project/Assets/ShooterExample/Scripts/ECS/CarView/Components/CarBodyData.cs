using Unity.Entities;
using Unity.Mathematics;

namespace Cars.View.Components
{
    public struct CarBodyData : IComponentData
    {
        public float3 Anchor;
        public float RotationDamping;
        public float MovementDamping;

        public float3 ParentForward;
        public float3 ParentRight;
        public float3 ParentUp;
    }
}