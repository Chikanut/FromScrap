using DamageSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace ForceField.Components
{
    public struct CreateForceFieldComponent : IComponentData
    {
        public float3 Position;
        
        public ForceFieldComponent ForceFieldInfo;
        public DealDamage DealDamageInfo;
        
        public CollisionFilter CollisionFilter;
    }
}