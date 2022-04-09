using System.Linq;
using DamageSystem.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(ResolveDamageSystem))]
    public partial class DamageCollisionSystem : SystemBase
    {
        private StepPhysicsWorld _stepPhysicsWorld;

        struct DamageTriggerJob : ITriggerEventsJob
        {
            public double Time;
            
            [ReadOnly] public BufferFromEntity<PhysicsColliderKeyEntityPair> ColliderPairs;
            [ReadOnly] public ComponentDataFromEntity<Dead> DeadGroup;
            
            public BufferFromEntity<Damage> DamageGroup;
            public ComponentDataFromEntity<DealDamage> DealDamageGroup;
            
            public void Execute(TriggerEvent triggerEvent)
            {
                var colliderEntityA = UpdateEntityWithChilds(triggerEvent.EntityA, triggerEvent.ColliderKeyA.Value);
                var colliderEntityB = UpdateEntityWithChilds(triggerEvent.EntityB, triggerEvent.ColliderKeyB.Value);

                CalculateDamage(colliderEntityA, colliderEntityB);
                CalculateDamage(colliderEntityB, colliderEntityA);
            }

            Entity UpdateEntityWithChilds(Entity entity, uint colliderKey)
            {
                if (!ColliderPairs.HasComponent(entity)) return entity;
                
                var colliderPairs = ColliderPairs[entity];
                for (int i = 0; i < colliderPairs.Length; i++)
                {
                    if (colliderPairs[i].Key.Value == colliderKey)
                    {
                        return colliderPairs[i].Entity;
                    }
                }

                return entity;
            }
            
            void CalculateDamage(Entity EntityA, Entity EntityB)
            {
                if(DeadGroup.HasComponent(EntityB)) return;
                if (!DealDamageGroup.HasComponent(EntityA) || DealDamageGroup[EntityA].isReloading) return;
                if (!DamageGroup.HasComponent(EntityB)) return;
                
                var dealDamage = DealDamageGroup[EntityA];
                DamageGroup[EntityB].Add(new Damage()
                {
                    Value = dealDamage.Value
                });
                    
                dealDamage.PrevHitTime = Time;
                DealDamageGroup[EntityA] = dealDamage;
            }
        }
        
        struct DamageCollisionJob : ICollisionEventsJob
        {
            public double Time;
            
            [ReadOnly] public BufferFromEntity<PhysicsColliderKeyEntityPair> ColliderPairs;
            [ReadOnly] public ComponentDataFromEntity<Dead> DeadGroup;
            
            public BufferFromEntity<Damage> DamageGroup;
            public ComponentDataFromEntity<DealDamage> DealDamageGroup;

            public void Execute(CollisionEvent triggerEvent)
            {
                var colliderEntityA = UpdateEntityWithChilds(triggerEvent.EntityA, triggerEvent.ColliderKeyA.Value);
                var colliderEntityB = UpdateEntityWithChilds(triggerEvent.EntityB, triggerEvent.ColliderKeyB.Value);

                CalculateDamage(colliderEntityA, colliderEntityB);
                CalculateDamage(colliderEntityB, colliderEntityA);
            }

            Entity UpdateEntityWithChilds(Entity entity, uint colliderKey)
            {
                if (!ColliderPairs.HasComponent(entity)) return entity;
                
                var colliderPairs = ColliderPairs[entity];
                for (int i = 0; i < colliderPairs.Length; i++)
                {
                    if (colliderPairs[i].Key.Value == colliderKey)
                    {
                        return colliderPairs[i].Entity;
                    }
                }

                return entity;
            }
            
            void CalculateDamage(Entity EntityA, Entity EntityB)
            {
                if(DeadGroup.HasComponent(EntityB)) return;
                if (!DealDamageGroup.HasComponent(EntityA) || DealDamageGroup[EntityA].isReloading) return;
                if (!DamageGroup.HasComponent(EntityB)) return;
                
                var dealDamage = DealDamageGroup[EntityA];
                DamageGroup[EntityB].Add(new Damage()
                {
                    Value = dealDamage.Value
                });
                    
                dealDamage.PrevHitTime = Time;
                DealDamageGroup[EntityA] = dealDamage;
            }
        }

        protected override void OnCreate()
        {
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

            base.OnCreate();
        }

        protected override void OnUpdate()
        {                                                        
            var damage = GetBufferFromEntity<Damage>();
            var colliderPair = GetBufferFromEntity<PhysicsColliderKeyEntityPair>(true);
            var dealDamage = GetComponentDataFromEntity<DealDamage>();
            var dead = GetComponentDataFromEntity<Dead>(true);
            var time = Time.ElapsedTime;
            
            var damageTriggerJob = new DamageTriggerJob()
            {
                DamageGroup = damage,
                DeadGroup = dead,
                DealDamageGroup = dealDamage,
                Time = time,
                ColliderPairs = colliderPair
            };
            var damageCollisionJob = new DamageCollisionJob()
            {
                DamageGroup = damage,
                DeadGroup = dead,
                DealDamageGroup = dealDamage,
                Time = time,
                ColliderPairs = colliderPair
            };
            
            damageTriggerJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
            damageCollisionJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
        }
    }
}