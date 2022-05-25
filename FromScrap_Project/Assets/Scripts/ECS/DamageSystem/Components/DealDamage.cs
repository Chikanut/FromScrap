using System;
using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct DealDamage : IComponentData
    {
        public int Value;

        public bool isPlayer;
        public float DamageDelay;
        public bool isReloading;
        public double PrevHitTime;
        
        public int MaxHits;
        public int CurrentHit;
    }
}
