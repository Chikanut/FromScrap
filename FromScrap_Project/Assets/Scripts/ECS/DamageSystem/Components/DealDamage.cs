using System;
using Unity.Entities;
using Unity.Physics;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct DealDamage : IComponentData
    {
        public float DamageDelay;
        public int Value;
        public bool isReloading;
        public double PrevHitTime;
        
    }
}
