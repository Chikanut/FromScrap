using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace StatisticsSystem.Systems
{
    [UpdateAfter(typeof(RecalculateCharacteristicsSystem))]
    public partial class InitializeCharacteristicsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var statisticsComponentFilter =
                GetComponentDataFromEntity<CharacteristicsComponent>(true);
            var parentFilter = GetComponentDataFromEntity<Parent>(true);
            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Dependency = Entities.WithAll<CharacteristicsComponent, Parent>().ForEach(
                (Entity entity, ref GetCharacteristicsTag getStatisticTag) =>
                {
                    var updated = UpdateStatistics(entity, entity, statisticsComponentFilter, parentFilter, ecb);

                   if(updated || getStatisticTag.TryUpdateTimes >= GetCharacteristicsTag.MaxTryUpdateTimes)  
                       ecb.RemoveComponent<GetCharacteristicsTag>(entity);
                   else
                       getStatisticTag.TryUpdateTimes++;

                }).WithReadOnly(parentFilter).WithReadOnly(statisticsComponentFilter).Schedule(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        static bool UpdateStatistics(Entity entity, Entity handler, ComponentDataFromEntity<CharacteristicsComponent> statisticsComponentFilter, ComponentDataFromEntity<Parent> parentFilter, EntityCommandBuffer ecb)
        {
            if (!parentFilter.HasComponent(entity))
                return false;

            var parent = parentFilter[entity].Value;

            if (!statisticsComponentFilter.HasComponent(parent))
                return parentFilter.HasComponent(parent) && UpdateStatistics(parent, handler, statisticsComponentFilter, parentFilter, ecb);
            
            ecb.SetComponent(handler, statisticsComponentFilter[parent]);
            ecb.AddComponent(handler, new CharacteristicsUpdatedTag());
            return true;

        }
    }
}