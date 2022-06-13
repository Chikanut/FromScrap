using ForceField.Components;
using Lifetime.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace ForceField.Systems
{
    public partial class CreateForceFieldSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;
        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBuffer.CreateCommandBuffer();
            var forceFieldArchetype = World.EntityManager.CreateArchetype(typeof(PhysicsWorldIndex));

            Dependency = Entities.ForEach((Entity entity, in CreateForceFieldComponent createComponent) =>
            {
                var forceField = ecb.CreateEntity(forceFieldArchetype);

                ecb.AddComponent(forceField, createComponent.ForceFieldInfo);

                if (createComponent.DealDamageInfo.Value > 0)
                    ecb.AddComponent(forceField, createComponent.DealDamageInfo);
                
                if(createComponent.ForceFieldInfo.LifeTime > 0)
                    ecb.AddComponent(forceField, new LifetimeComponent()
                    {
                        CallDeathEvent = false,
                        MaxLifeTime = createComponent.ForceFieldInfo.LifeTime
                    });

                ecb.AddComponent<LocalToWorld>(forceField, new LocalToWorld());
                ecb.AddComponent(forceField, new Translation() {Value = createComponent.Position});
                
                var radius = createComponent.ForceFieldInfo.Radius;
                
                var sphere = new SphereGeometry
                {
                    Center = float3.zero,
                    Radius = createComponent.ForceFieldInfo.Radius
                };
                
                var material = Material.Default;
                
                material.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;
                
                var colliderRef = SphereCollider.Create(sphere, createComponent.CollisionFilter, material);

                ecb.AddComponent(forceField, new PhysicsCollider {Value = colliderRef});
                ecb.AddComponent(forceField, new SphereTriggerComponent() {Radius = radius, PrevRadius = radius});
                ecb.AddBuffer<StatefulTriggerEvent>(forceField);
                
                ecb.DestroyEntity(entity);
            }).Schedule(Dependency);
            
            _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}