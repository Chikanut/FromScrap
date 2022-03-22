using DamageSystem.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(ResolveDamageSystem))]
    public partial class DamageCollisionSystem : SystemBase
    {
        private StepPhysicsWorld _stepPhysicsWorld;

        struct DamageTriggerJob : ITriggerEventsJob
        {
            public BufferFromEntity<Damage> DamageGroup;
            [ReadOnly] public ComponentDataFromEntity<Dead> DeadGroup;
            [ReadOnly] public ComponentDataFromEntity<DealDamage> DealDamageGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                CalculateDamage(triggerEvent.EntityA, triggerEvent.EntityB);
                CalculateDamage(triggerEvent.EntityB, triggerEvent.EntityA);
            }

            void CalculateDamage(Entity EntityA, Entity EntityB)
            {
                if(DeadGroup.HasComponent(EntityB)) return;
                if (!DealDamageGroup.HasComponent(EntityA)) return;
                
                if (DamageGroup.HasComponent(EntityB))
                {
                    DamageGroup[EntityB].Add(new Damage()
                    {
                        Value = DealDamageGroup[EntityA].Value
                    });
                }
            }
        }
        
        struct DamageCollisionJob : ICollisionEventsJob
        {
            public BufferFromEntity<Damage> DamageGroup;
            [ReadOnly] public ComponentDataFromEntity<Dead> DeadGroup;
            [ReadOnly] public ComponentDataFromEntity<DealDamage> DealDamageGroup;

            public void Execute(CollisionEvent triggerEvent)
            {
                CalculateDamage(triggerEvent.EntityA, triggerEvent.EntityB);
                CalculateDamage(triggerEvent.EntityB, triggerEvent.EntityA);
            }

            void CalculateDamage(Entity EntityA, Entity EntityB)
            {
                if(DeadGroup.HasComponent(EntityB)) return;
                if (!DealDamageGroup.HasComponent(EntityA)) return;
                
                if (DamageGroup.HasComponent(EntityB))
                {
                    DamageGroup[EntityB].Add(new Damage()
                    {
                        Value = DealDamageGroup[EntityA].Value
                    });
                }
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
            var dealDamage = GetComponentDataFromEntity<DealDamage>(true);
            var dead = GetComponentDataFromEntity<Dead>(true);
            
            var damageTriggerJob = new DamageTriggerJob()
            {
                DamageGroup = damage,
                DeadGroup = dead,
                DealDamageGroup = dealDamage
            };
            var damageCollisionJob = new DamageCollisionJob()
            {
                DamageGroup = damage,
                DeadGroup = dead,
                DealDamageGroup = dealDamage
            };
            
            damageTriggerJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
            damageCollisionJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
        }
    }
}