using DamageSystem.Components;
using DamageSystem.Systems;
using Magnet.Components;
using MoveTo.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Magnet.Systems
{
    [UpdateInGroup(typeof (SimulationSystemGroup), OrderFirst = true), UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial class MagnetSystem : SystemBase
    {
        private StepPhysicsWorld _stepPhysicsWorld;
        private BeginSimulationEntityCommandBufferSystem _ecbSystem;
        private JobHandle _bufferSetJobHandle;

        protected override void OnCreate()
        {
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            _ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var colliderPair = GetBufferFromEntity<PhysicsColliderKeyEntityPair>(true);
            var magnets = GetComponentDataFromEntity<MagnetComponent>(true);
            var moveTo = GetComponentDataFromEntity<MoveToComponent>(true);
            var magnetTargets = GetBufferFromEntity<MagnetTargetsBuffer>();
            
            var magnetTriggerJob = new MagnetTriggerJob()
            {
                MagnetTargets = magnetTargets,
                Magnets = magnets,
                MoveToComponents = moveTo,
                ColliderPairs = colliderPair
            };
            
            Dependency = magnetTriggerJob.Schedule(_stepPhysicsWorld.Simulation,
                JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle));
            
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            var dead = GetComponentDataFromEntity<Dead>();

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex,
                ref DynamicBuffer<MagnetTargetsBuffer> magnetTargets, in MagnetComponent magnet) =>
            {
                foreach (var magnetTarget in magnetTargets)
                {
                    if(!dead.HasComponent(magnetTarget.Target))
                        ecb.AddComponent(entityInQueryIndex, magnetTarget.Target, new MoveToComponent() {Target = entity, Speed = magnet.Speed});
                }

                magnetTargets.Clear();
            }).WithReadOnly(dead).ScheduleParallel(Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }

        
        struct MagnetTriggerJob : ITriggerEventsJob
        {
            [ReadOnly] public BufferFromEntity<PhysicsColliderKeyEntityPair> ColliderPairs;
            [ReadOnly] public ComponentDataFromEntity<MagnetComponent> Magnets;
            [ReadOnly] public ComponentDataFromEntity<MoveToComponent> MoveToComponents;
            public BufferFromEntity<MagnetTargetsBuffer> MagnetTargets;
        
        
            public void Execute(TriggerEvent triggerEvent)
            {
                var colliderEntityA = UpdateEntityWithChilds(triggerEvent.EntityA, triggerEvent.ColliderKeyA.Value);
                var colliderEntityB = UpdateEntityWithChilds(triggerEvent.EntityB, triggerEvent.ColliderKeyB.Value);
        
                TryMagnetObjects(colliderEntityA, colliderEntityB);
                TryMagnetObjects(colliderEntityB, colliderEntityA);
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
        
            void TryMagnetObjects(Entity EntityA, Entity EntityB)
            {
                if(!Magnets.HasComponent(EntityA) || !MagnetTargets.HasComponent(EntityA) || MoveToComponents.HasComponent(EntityB)) return;
                
                MagnetTargets[EntityA].Add(new MagnetTargetsBuffer() {Target = EntityB});
            }
        }
    }
}