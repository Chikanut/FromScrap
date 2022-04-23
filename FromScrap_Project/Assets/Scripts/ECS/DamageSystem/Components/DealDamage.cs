using System;
using Unity.Entities;
using Unity.Physics;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct DealDamage : IComponentData
    {
        public int Value;
        
        public float DamageDelay;
        public bool isReloading;
        public double PrevHitTime;
        
        public int MaxHits;
        public int CurrentHit;
    }
}
