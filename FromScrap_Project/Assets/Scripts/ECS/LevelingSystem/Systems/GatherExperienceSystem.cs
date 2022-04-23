using DamageSystem.Components;
using LevelingSystem.Components;
using Unity.Entities;
using Unity.Physics.Stateful;

namespace LevelingSystem.Systems
{
    public partial class GatherExperienceSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
           var experienceBuffer = GetBufferFromEntity<AddExperienceBuffer>();
           var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

           Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, in ExperienceComponent experience, in DynamicBuffer<StatefulTriggerEvent> triggerEvents) =>
           {
               foreach (var triggerEvent in triggerEvents)
               {
                   if(triggerEvent.State != EventOverlapState.Enter) continue;

                   var otherEntity = triggerEvent.GetOtherEntity(entity);
                   
                   if (!experienceBuffer.HasComponent(otherEntity)) continue;
                   
                   experienceBuffer[otherEntity].Add(new AddExperienceBuffer()
                       {Value = experience.Value});
                   
                   ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                   
                   break;

               }
           }).Schedule(Dependency);

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}