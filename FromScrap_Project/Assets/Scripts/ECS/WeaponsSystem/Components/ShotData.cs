using DamageSystem.Components;
using ForceField.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct ShotData : IComponentData
    {
        public float Velocity;
        public float Gravity;
        public float SpeedDamping;
        public float DirectionDamping;

        public ForceFieldComponent HitForce;
        public DealDamage HitForceDamage;
    }
}