using Collectables.Components;
using DamageSystem.Components;
using LevelingSystem.Components;
using Unity.Entities;

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

           Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, in ExperienceComponent experience, in CollectableGatheredComponent collectedInfo) =>
           {
               if (!experienceBuffer.HasComponent(collectedInfo.CollectedEntity)) return;
               
               experienceBuffer[collectedInfo.CollectedEntity].Add(new AddExperienceBuffer()
                   {Value = experience.Value});

               ecb.AddComponent(entityInQueryIndex, entity, new Dead());

           }).Schedule(Dependency);

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}