using BovineLabs.Event.Systems;
using DamageSystem.Components;
using LevelingSystem.Components;
using SpawnGameObjects.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace LevelingSystem.Systems
{
    public partial class GatherExperienceSystem : SystemBase
    {
        private StepPhysicsWorld _stepPhysicsWorld;
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        
        struct ExperienceTriggerJob : ITriggerEventsJob
        {
            [ReadOnly] public BufferFromEntity<PhysicsColliderKeyEntityPair> ColliderPairs;
            public ComponentDataFromEntity<ExperienceComponent> ExperienceComponent;
            public BufferFromEntity<AddExperienceBuffer> AddExperienceBuffer;
          
            
            public void Execute(TriggerEvent triggerEvent)
            {
                var colliderEntityA = UpdateEntityWithChilds(triggerEvent.EntityA, triggerEvent.ColliderKeyA.Value);
                var colliderEntityB = UpdateEntityWithChilds(triggerEvent.EntityB, triggerEvent.ColliderKeyB.Value);

                CalculateExperience(colliderEntityA, colliderEntityB);
                CalculateExperience(colliderEntityB, colliderEntityA);
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
            
            void CalculateExperience(Entity EntityA, Entity EntityB)
            {
                if (!ExperienceComponent.HasComponent(EntityA)) return;
                if (!AddExperienceBuffer.HasComponent(EntityB)) return;
                
                var addExperience = ExperienceComponent[EntityA];
                if(addExperience.Gathered) return;
                
                AddExperienceBuffer[EntityB].Add(new AddExperienceBuffer()
                {
                    Value = addExperience.Value
                });

                addExperience.Gathered = true;
                ExperienceComponent[EntityA] = addExperience;
            }
        }
        
        struct ExperienceCollisionJob : ICollisionEventsJob
        {
            [ReadOnly] public BufferFromEntity<PhysicsColliderKeyEntityPair> ColliderPairs;
            [ReadOnly] public ComponentDataFromEntity<ExperienceComponent> ExperienceComponent;
            public BufferFromEntity<AddExperienceBuffer> AddExperienceBuffer;
          
            public void Execute(CollisionEvent collisionEvent)
            {
                var colliderEntityA = UpdateEntityWithChilds(collisionEvent.EntityA, collisionEvent.ColliderKeyA.Value);
                var colliderEntityB = UpdateEntityWithChilds(collisionEvent.EntityB, collisionEvent.ColliderKeyB.Value);

                CalculateExperience(colliderEntityA, colliderEntityB);
                CalculateExperience(colliderEntityB, colliderEntityA);
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
            
            void CalculateExperience(Entity EntityA, Entity EntityB)
            {
                if (!ExperienceComponent.HasComponent(EntityA)) return;
                if (!AddExperienceBuffer.HasComponent(EntityB)) return;
                
                var addExperience = ExperienceComponent[EntityA];
                AddExperienceBuffer[EntityB].Add(new AddExperienceBuffer()
                {
                    Value = addExperience.Value
                });
                
            }
        }
        
      
        
        protected override void OnCreate()
        {
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }
        
        
        protected override void OnUpdate()
        {                                                        
            var addExperienceBuffer = GetBufferFromEntity<AddExperienceBuffer>();
            var colliderPair = GetBufferFromEntity<PhysicsColliderKeyEntityPair>(true);
            var experienceComponent = GetComponentDataFromEntity<ExperienceComponent>();

            var experienceTriggerJob = new ExperienceTriggerJob()
            {
                AddExperienceBuffer = addExperienceBuffer,
                ExperienceComponent = experienceComponent,
                ColliderPairs = colliderPair
            };
            var experienceCollisionJob = new ExperienceCollisionJob()
            {
                AddExperienceBuffer = addExperienceBuffer,
                ExperienceComponent = experienceComponent,
                ColliderPairs = colliderPair
            };

            experienceTriggerJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
            experienceCollisionJob.Schedule(_stepPhysicsWorld.Simulation, JobHandle.CombineDependencies(Dependency, _stepPhysicsWorld.FinalSimulationJobHandle)).Complete();
            
          
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex , in ExperienceComponent experience, in LocalToWorld localToWorld) =>
            {
                if (experience.Gathered)
                {
                    ecb.AddComponent<Dead>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
            
            _ecbSystem.AddJobHandleForProducer (Dependency);
        }
    }
}