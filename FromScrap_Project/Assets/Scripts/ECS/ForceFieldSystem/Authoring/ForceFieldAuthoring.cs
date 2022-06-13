using DamageSystem.Components;
using ForceField.Components;
using Lifetime.Components;
using Unity.Entities;
using Unity.Physics.Stateful;
using UnityEngine;

namespace ForceField.Authorings
{
    public class ForceFieldAuthoring : MonoBehaviour , IConvertGameObjectToEntity
    {
        [Header("Base")]
        public float Radius;
        public float Force;
        public bool ForceIn;
        public float LifeTime = 0;
        
        [Header("Damage")]
        public int Damage;
        public bool isPlayer;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new ForceFieldComponent()
            {
                Radius = Radius,
                Force = Force
            });

            if (Damage > 0)
            {
                dstManager.AddComponentData(entity, new DealDamage()
                {
                    isPlayer = isPlayer,
                    Value = Damage,
                    CurrentHit = 0,
                    DamageDelay = 0,
                    isReloading = false,
                    MaxHits = 1,
                    PrevHitTime = 0
                });
            }
            
            if(LifeTime > 0)
                dstManager.AddComponentData(entity, new LifetimeComponent()
                {
                    CallDeathEvent = false,
                    MaxLifeTime = LifeTime
                });
            
            dstManager.AddBuffer<StatefulTriggerEvent>(entity);
        }
    }
}