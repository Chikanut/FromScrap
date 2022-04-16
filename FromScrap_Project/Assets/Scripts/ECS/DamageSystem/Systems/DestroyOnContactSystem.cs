using DamageSystem.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(SpawnOnDeathSystem))]
    public partial class DestroyOnContactSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        private StepPhysicsWorld _stepPhysicsWorld;
        
        struct DestroyTriggerJob : ITriggerEventsJob
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public ComponentDataFromEntity<DestroyOnContact> destroyOnContactGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Calculate(triggerEvent.EntityA);
                Calculate(triggerEvent.EntityB);
            }

            void Calculate(Entity entity)
            {
                if(destroyOnContactGroup.HasComponent(entity))
                    ecb.AddComponent<Dead>(entity);
            }
        }
        
        struct DestroyCollisionJob : ICollisionEventsJob
        {
            public EntityCommandBuffer ecb;
            [ReadOnly] public ComponentDataFromEntity<DestroyOnContact> destroyOnContactGroup;

            public void Execute(CollisionEvent triggerEvent)
            {
                Calculate(triggerEvent.EntityA);
                Calculate(triggerEvent.EntityB);
            }

            void Calculate(Entity entity)
            {
                if(destroyOnContactGroup.HasComponent(entity))
                    ecb.AddComponent<Dead>(entity);
            }
        }

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var destroyOnContactGroup = GetComponentDataFromEntity<DestroyOnContact>(true);
            var ecb = _ecbSystem.CreateCommandBuffer();

            var destroyTriggerJob = new DestroyTriggerJob()
            {
                ecb = ecb,
                destroyOnContactGroup = destroyOnContactGroup
            };
            
            var destroyCollisionJob = new DestroyCollisionJob()
            {
                ecb = ecb,
                destroyOnContactGroup = destroyOnContactGroup
            };
            
            destroyTriggerJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
            destroyCollisionJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
        }
    }
}