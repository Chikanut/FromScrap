using DamageSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace DamageSystem.Systems.DamageHighLight
{
    public partial class DamageHighLightProccessingSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = Time.DeltaTime;

           Dependency = Entities.WithNone<Dead>().ForEach((Entity entity, int entityInQueryIndex, ref URPMaterialPropertyBaseColor color, ref DamageHighLightComponent highLightComponent) =>
            {
                highLightComponent.HighLightTimeLeft -= deltaTime;

                if (highLightComponent.HighLightTimeLeft <= 0)
                {
                    ecb.RemoveComponent<DamageHighLightComponent>(entityInQueryIndex, entity);
                    color.Value = new float4(1, 1, 1, 1);
                }

            }).ScheduleParallel(Dependency);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        }
    }
    
    [UpdateBefore(typeof(ResolveDamageSystem)), UpdateAfter(typeof(DamageCollisionSystem))]
    public partial class AddDamageHighLightSystem : SystemBase
    {
        const float HighLightTime = 2;
        
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var damageHighLightFilter = GetComponentDataFromEntity<DamageHighLightComponent>(true);
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Dependency = Entities.WithNone<Dead>().ForEach((int entityInQueryIndex, in DynamicBuffer<Damage> damages, in DynamicBuffer<DamageHighLightMeshBuffer> highLightMeshes) =>
            {
                if (damages.Length <= 0) return;
                
                for (var i = 0; i < highLightMeshes.Length; i++)
                {
                    if (damageHighLightFilter.HasComponent(highLightMeshes[i].Entity))
                        ecb.SetComponent(entityInQueryIndex, highLightMeshes[i].Entity, new DamageHighLightComponent(){HighLightMaxTime = HighLightTime, HighLightTimeLeft = HighLightTime});
                    else
                    {
                        ecb.AddComponent(entityInQueryIndex, highLightMeshes[i].Entity,
                            new DamageHighLightComponent()
                                {HighLightMaxTime = HighLightTime, HighLightTimeLeft = HighLightTime});
                    }
                }
            }).WithReadOnly(damageHighLightFilter).ScheduleParallel(Dependency);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    
    public partial class DamageHighLightSystem : SystemBase
    {
        const float HighLightFrequency = 10;
    
        protected override void OnUpdate()
        {
            Entities.ForEach((ref URPMaterialPropertyBaseColor color, in DamageHighLightComponent damageHighLight) =>
            {
                var newColor = Color.Lerp(Color.white, Color.red,
                    math.sin(damageHighLight.HighLightTimeLeft * HighLightFrequency));
                color.Value = new float4(newColor.r, newColor.g, newColor.b, newColor.a);
            }).ScheduleParallel();
        }
    }
}